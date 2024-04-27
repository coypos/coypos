<template>
  <!-- Modal -->
  <div
    class="modal fade"
    id="staticBackdrop"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
    tabindex="-1"
    aria-labelledby="staticBackdropLabel"
    aria-hidden="true"
  >
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h1 v-if="!create" class="modal-title" id="staticBackdropLabel">
            Edytuj płatność
          </h1>
          <h1 v-else class="modal-title" id="staticBackdropLabel">
            Dodaj płatność
          </h1>
        </div>
        <div v-if="editpayment_method" class="modal-body">
          <div class="row">
            <div class="col-8">
              <div class="row">
                <div class="col-4">Język</div>
                <div class="col-4">Nazwa</div>
              </div>
              <div class="row" v-for="name in names" :key="name">
                <div class="col-4">
                  <select
                    class="form-control selectpicker"
                    id="category"
                    data-live-search="true"
                    v-model="name.lang"
                  >
                    <option :data-tokens="null">Brak</option>
                    <option
                      :key="language.id"
                      v-for="language in languages"
                      :data-tokens="language.countryCode"
                      :value="language.countryCode"
                    >
                      {{ language.name }}
                    </option>
                  </select>
                </div>
                <div class="col-4"><input v-model="name.name" /></div>
                <div class="col-4">
                  <button
                    class="btn btn-danger"
                    @click="deleteName(name.lang, name.name)"
                  >
                    Usuń
                  </button>
                </div>
              </div>
              <button class="btn-success btn" @click="addName()">Dodaj</button>
            </div>

            <div class="col-3">
              <div class="form-group">
                <label for="value">Dane autoryzacyjne</label>

                <div
                  :class="{
                    error: v$.editpayment_method.authData.$errors.length,
                  }"
                >
                  <input
                    v-model="editpayment_method.authData"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.editpayment_method.authData.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <div class="form-check">
                  <input
                    v-model="editpayment_method.enabled"
                    type="checkbox"
                    class="form-check-input"
                    id="enabled"
                  />
                  <label class="form-check-label" for="exampleCheck1"
                    >Czy aktywna?</label
                  >
                </div>
              </div>
            </div>
          </div>

          <div class="row">
            <div class="col-6">
              <div class="form-group">
                <label for="image">Zdjęcie</label>
                <input
                  type="file"
                  ref="photo"
                  accept="image/*"
                  @change="encodeImageFileAsURL()"
                  class="form-control"
                  id="image"
                />
                <div
                  class="input-errors"
                  v-for="error of v$.editpayment_method.image.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
              </div>
            </div>
            <div class="col-6">
              <img
                id="base64image"
                :src="'data:image/jpeg;base64,' + editpayment_method.image"
              />
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button
            type="button"
            :class="'btn btn-warning'"
            data-bs-dismiss="modal"
            @click="canceladd()"
          >
            ANULUJ
          </button>
          <button
            :disabled="
              buttondisabled ||
              v$.editpayment_method.$errors.length ||
              v$.names.$errors.length
            "
            type="button"
            :class="'btn btn-success'"
            data-bs-dismiss="modal"
            @click="updatePaymentMethod()"
          >
            ZAPISZ
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { PaymentMethodModel } from "@/types/api/PaymentMethod";
import { useVuelidate } from "@vuelidate/core";
import { useToast, POSITION } from "vue-toastification";
import { required, minLength } from "@vuelidate/validators";
import { compressImage, resizePNG } from "@/functions";
import { LanguageModel } from "@/types/api/Language";
import { LanguageNamesModal } from "@/types/LanguageNames";
import { ResponseModel } from "@/types/Response";

export default defineComponent({
  props: {
    payment_method: Object,
    create: Boolean,
  },

  expose: ["showModal"],
  name: "PaymentMethodModal",
  setup() {
    const toast = useToast();
    let keys = ref<string[]>();
    let editpayment_method = ref<PaymentMethodModel>({
      id: null,
      name: null,
      image: null,
      authData: null,
      enabled: false,
    });
    let buttondisabled = ref<boolean>(false);
    let languages = ref<LanguageModel[]>([]);
    let names = ref<LanguageNamesModal[]>([]);

    const v$ = useVuelidate();
    return {
      names,
      languages,
      buttondisabled,
      toast,
      v$,
      keys,
      editpayment_method,
    };
  },
  validations() {
    return {
      editpayment_method: {
        name: { $autoDirty: true },
        image: { $autoDirty: true },
        authData: { $autoDirty: true },
        enabled: { $autoDirty: true },
      },
      names: [
        {
          lang: { required, $autoDirty: true },
          name: { required, minLength: minLength(3), $autoDirty: true },
        },
      ],
    };
  },
  methods: {
    async getLanguages() {
      try {
        await this.$axios
          .get(`/languages?filter=AND&itemsPerPage=999999&page=1`)
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.languages = resp.response;
          });
      } catch (e: any) {
        this.toast.error(e.code, {
          position: "top-right" as POSITION,
          timeout: 5000,
          closeOnClick: true,
          pauseOnFocusLoss: true,
          pauseOnHover: true,
          draggable: true,
          draggablePercent: 0.6,
          showCloseButtonOnHover: false,
          hideProgressBar: true,
          closeButton: "button",
          icon: true,
          rtl: false,
        });
      }
    },
    async getNames() {
      this.names = [];
      if (this.payment_method) {
        if (this.payment_method.name) {
          let lang = this.payment_method.name.split("|");
          for (let i = 0; lang.length > i; i++) {
            this.names.push({
              name: lang[i].split(":")[1] as string,
              lang: (lang[i].split(":")[0] as string).toLowerCase(),
            });
          }
        }
      }
    },
    async deleteName(lang: string, name: string) {
      this.names = this.names.filter((name2) => {
        return name2.name !== name;
      });
    },
    async addName() {
      this.names.push({ name: "", lang: "" });
    },
    async encodeImageFileAsURL() {
      this.buttondisabled = true;
      const element: HTMLInputElement = this.$refs.photo as HTMLInputElement;
      if (element.files) {
        const file = element.files[0];
        if (file) {
          if (file.type == "image/png") {
            const targetSizeInKB = 20;
            this.editpayment_method.image = (
              await resizePNG(file, targetSizeInKB)
            ).substring("data:image/png;base64,".length);
          } else {
            const reader = new FileReader();
            reader.onloadend = async () => {
              const originalImageBase64 = reader.result as string;
              const maxSizeInBytes = 1024 * 10;
              const maxWidth = 720;
              this.editpayment_method.image = await compressImage(
                originalImageBase64,
                maxSizeInBytes,
                maxWidth
              );
            };
            reader.readAsDataURL(file);
          }
          this.buttondisabled = false;
        }
      }
    },
    async updatePaymentMethod() {
      let tempname = "";
      for (let i = 0; this.names.length > i; i++) {
        if (this.names[i]) {
          tempname += `${this.names[i].lang}:${this.names[i].name}|`;
        }
      }
      tempname = tempname.slice(0, -1);

      let data = {
        name: tempname,
        image: this.editpayment_method.image,
        authData: this.editpayment_method.authData,
        enabled: this.editpayment_method.enabled,
      };
      if (this.create) {
        try {
          await this.$axios.post(`/payment_method`, data);
          this.toast.success("Utworzono metode płatności", {
            position: "top-right" as POSITION,
            timeout: 5000,
            closeOnClick: true,
            pauseOnFocusLoss: true,
            pauseOnHover: true,
            draggable: true,
            draggablePercent: 0.6,
            showCloseButtonOnHover: false,
            hideProgressBar: true,
            closeButton: "button",
            icon: true,
            rtl: false,
          });
        } catch (e: any) {
          this.toast.error(e.code, {
            position: "top-right" as POSITION,
            timeout: 5000,
            closeOnClick: true,
            pauseOnFocusLoss: true,
            pauseOnHover: true,
            draggable: true,
            draggablePercent: 0.6,
            showCloseButtonOnHover: false,
            hideProgressBar: true,
            closeButton: "button",
            icon: true,
            rtl: false,
          });
        }
      } else {
        try {
          await this.$axios.put(
            `/payment_method/${this.editpayment_method.id}`,
            data
          );
          this.toast.success("Zedytowano metode płatności", {
            position: "top-right" as POSITION,
            timeout: 5000,
            closeOnClick: true,
            pauseOnFocusLoss: true,
            pauseOnHover: true,
            draggable: true,
            draggablePercent: 0.6,
            showCloseButtonOnHover: false,
            hideProgressBar: true,
            closeButton: "button",
            icon: true,
            rtl: false,
          });
        } catch (e: any) {
          this.toast.error(e.code, {
            position: "top-right" as POSITION,
            timeout: 5000,
            closeOnClick: true,
            pauseOnFocusLoss: true,
            pauseOnHover: true,
            draggable: true,
            draggablePercent: 0.6,
            showCloseButtonOnHover: false,
            hideProgressBar: true,
            closeButton: "button",
            icon: true,
            rtl: false,
          });
        }
      }
      this.editpayment_method = {
        id: null,
        name: null,
        image: null,
        authData: null,
        enabled: false,
      };
      this.$emit("refreshpayment_methods", true);
    },

    async canceladd() {
      this.editpayment_method = {
        id: null,
        name: null,
        image: null,
        authData: null,
        enabled: false,
      };

      this.$emit("canceladd", false);
    },
  },
  mounted() {
    this.getNames();
    this.getLanguages();
    this.v$.$validate();
  },
  watch: {
    payment_method(value, newvalue) {
      this.getLanguages();

      this.getNames();
      this.editpayment_method = this.payment_method as PaymentMethodModel;
    },
  },
});
</script>
<style scoped lang="scss">
.modal {
  padding-top: 200px;
  --bs-modal-width: 80%;
  .modal-content {
    color: #fdfdfd;
    background-color: #1f1f1f;
    .modal-body {
      font-size: 17px;
      padding: 50px 10px;
      .input-errors {
        color: red;
        font-size: 0.7em;
      }
      #base64image {
        margin: 20px;
        display: block;
        width: auto;
        height: 150px;
      }
    }
    .modal-footer {
      font-size: 20px;
      button {
        font-size: 20px;
        padding: 20px;
      }
    }
  }
}
</style>
