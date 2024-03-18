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
            Edytuj język
          </h1>
          <h1 v-else class="modal-title" id="staticBackdropLabel">
            Dodaj jęzuk
          </h1>
        </div>
        <div v-if="editlanguage" class="modal-body">
          <div class="row">
            <div class="col-3">
              <div class="form-group">
                <label for="value">Nazwa</label>

                <div :class="{ error: v$.editlanguage.name.$errors.length }">
                  <input
                    v-model="editlanguage.name"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.editlanguage.name.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
              </div>
            </div>

            <div class="col-3">
              <div class="form-group">
                <label for="value">Skrót</label>

                <div
                  :class="{ error: v$.editlanguage.countryCode.$errors.length }"
                >
                  <input
                    v-model="editlanguage.countryCode"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.editlanguage.countryCode.$errors"
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
                    v-model="editlanguage.enabled"
                    type="checkbox"
                    class="form-check-input"
                    id="enabled"
                  />
                  <label class="form-check-label" for="exampleCheck1"
                    >Czy aktywny?</label
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
                  v-for="error of v$.editlanguage.image.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
              </div>
            </div>
            <div class="col-6">
              <img
                id="base64image"
                :src="'data:image/jpeg;base64,' + editlanguage.image"
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
            :disabled="v$.editlanguage.$errors.length"
            type="button"
            :class="'btn btn-success'"
            data-bs-dismiss="modal"
            @click="updateLanguage()"
          >
            ZAPISZ
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref, reactive } from "vue";
import { LanguageModel } from "@/types/api/Language";
import { useVuelidate } from "@vuelidate/core";
import { useToast, POSITION } from "vue-toastification";
import { required, minLength } from "@vuelidate/validators";
import { compressImage, resizePNG } from "@/functions";

export default defineComponent({
  props: {
    language: Object,
    create: Boolean,
  },

  expose: ["showModal"],
  name: "LanguageModal",
  setup() {
    const toast = useToast();
    let keys = ref<string[]>();
    let editlanguage = ref<LanguageModel>({
      id: null,
      name: null,
      image: null,
      enabled: null,
      countryCode: null,
    });

    const v$ = useVuelidate();
    let buttondisabled = ref<boolean>(false);

    return { toast, v$, keys, editlanguage, buttondisabled };
  },
  validations() {
    return {
      editlanguage: {
        name: { required, minLength: minLength(3), $autoDirty: true },
        countryCode: { required, $autoDirty: true },
        enabled: { $autoDirty: true },
        image: { required, $autoDirty: true },
      },
    };
  },
  methods: {
    async encodeImageFileAsURL() {
      this.buttondisabled = true;
      const element: HTMLInputElement = this.$refs.photo as HTMLInputElement;
      if (element.files) {
        const file = element.files[0];
        if (file) {
          if (file.type == "image/png") {
            const targetSizeInKB = 20;
            this.editlanguage.image = (
              await resizePNG(file, targetSizeInKB)
            ).substring("data:image/png;base64,".length);
          } else {
            const reader = new FileReader();
            reader.onloadend = async () => {
              const originalImageBase64 = reader.result as string;
              const maxSizeInBytes = 1024 * 10;
              const maxWidth = 720;
              this.editlanguage.image = await compressImage(
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
    async updateLanguage() {
      let data = {
        name: this.editlanguage.name,
        countryCode: this.editlanguage.countryCode,
        enabled: this.editlanguage.enabled,
        image: this.editlanguage.image,
      };
      if (this.create) {
        try {
          await this.$axios.post(`/language`, data);
          this.toast.success("Utworzono użytkownika", {
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
          await this.$axios.put(`/language/${this.editlanguage.id}`, data);
          this.toast.success("Zedytowano użytkownika", {
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
      this.editlanguage = {
        id: null,
        name: null,
        image: null,
        enabled: null,
        countryCode: null,
      };
      this.$emit("refreshlanguages", true);
    },

    async canceladd() {
      this.editlanguage = {
        id: null,
        name: null,
        image: null,
        enabled: null,
        countryCode: null,
      };

      this.$emit("canceladd", false);
    },
  },
  mounted() {
    this.v$.$validate();
  },
  watch: {
    // whenever question changes, this function will run
    language(value, newvalue) {
      this.editlanguage = this.language as LanguageModel;
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
