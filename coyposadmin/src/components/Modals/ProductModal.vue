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
            Edytuj produkt
          </h1>
          <h1 v-else class="modal-title" id="staticBackdropLabel">
            Dodaj produkt
          </h1>
        </div>
        <div v-if="editproduct" class="modal-body">
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
          </div>
          <div class="row">
            <div class="col-6">
              <div class="form-group">
                <label for="category">Kategoria</label>
                <select
                  class="form-control selectpicker"
                  id="category"
                  data-live-search="true"
                  v-model="editproduct.category.id"
                >
                  <option
                    :key="category.id"
                    v-for="category in categories"
                    :data-tokens="category.name"
                    :value="category.id"
                  >
                    {{ category.name }}
                  </option>
                </select>
                <div
                  class="input-errors"
                  v-for="error of v$.editproduct.category.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-6">
              <div class="form-group">
                <label for="barcode">Kod kreskowy</label>
                <input
                  type="number"
                  v-model="editproduct.barcode"
                  class="form-control"
                  id="barcode"
                />
                <div
                  class="input-errors"
                  v-for="error of v$.editproduct.barcode.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="price">Cena</label>
                <input
                  v-model="editproduct.price"
                  class="form-control"
                  id="price"
                />
                <div
                  class="input-errors"
                  v-for="error of v$.editproduct.price.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="weight">Waga</label>
                <input
                  v-model="editproduct.weight"
                  class="form-control"
                  id="weight"
                />
              </div>
              <div
                class="input-errors"
                v-for="error of v$.editproduct.weight.$errors"
                :key="error.$uid"
              >
                <div class="error-msg">{{ error.$message }}</div>
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-12">
              <div class="form-group">
                <label for="description">Opis</label>
                <input
                  v-model="editproduct.description"
                  class="form-control"
                  id="description"
                />
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
                  v-for="error of v$.editproduct.image.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
              </div>
            </div>
            <div class="col-6">
              <img
                id="base64image"
                :src="'data:image/jpeg;base64,' + editproduct.image"
              />
            </div>
          </div>
          <div class="row">
            <div class="col-6">
              <div class="form-check">
                <input
                  v-model="editproduct.isLoose"
                  type="checkbox"
                  class="form-check-input"
                  id="isLoose"
                />
                <label class="form-check-label" for="exampleCheck1"
                  >Na wagę?</label
                >
              </div>
            </div>
            <div class="col-6">
              <div class="form-check">
                <input
                  v-model="editproduct.enabled"
                  type="checkbox"
                  class="form-check-input"
                  id="enabled"
                />
                <label class="form-check-label" for="exampleCheck1"
                  >Włączony?</label
                >
              </div>
            </div>
            <div class="row">
              <div class="col-6">
                <div class="form-check">
                  <input
                    v-model="editproduct.ageRestricted"
                    type="checkbox"
                    class="form-check-input"
                    id="enabled"
                  />
                  <label class="form-check-label" for="exampleCheck1"
                    >18+?</label
                  >
                </div>
              </div>
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
            type="button"
            :class="'btn btn-success'"
            data-bs-dismiss="modal"
            @click="updateProduct()"
            :disabled="buttondisabled || v$.editproduct.$errors.length"
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
import { ProductModel } from "@/types/api/Product";
import { CategoryModel } from "@/types/api/Category";
import { ResponseModel } from "@/types/Response";
import { compressImage } from "@/functions";
import { POSITION, useToast } from "vue-toastification";
import { useVuelidate } from "@vuelidate/core";
import { required, numeric } from "@vuelidate/validators";
import { LanguageNamesModal } from "@/types/LanguageNames";
import { LanguageModel } from "@/types/api/Language";
export default defineComponent({
  props: {
    product: Object,
    create: Boolean,
  },
  expose: ["showModal"],
  name: "ProductModal",
  setup() {
    let keys = ref<string[]>();
    let categories = ref<CategoryModel[]>([]);
    let languages = ref<LanguageModel[]>([]);

    let editproduct = ref<ProductModel>({
      id: null,
      createDate: null,
      updateDate: null,
      enabled: false,
      name: null,
      barcode: null,
      price: null,
      isLoose: false,
      weight: null,
      description: null,
      category: {
        isVisible: null,
        image: null,
        name: "",
        id: 0,
        parentCategory: null,
        updateDate: null,
        createDate: null,
      },
      image: null,
      discountedPrice: null,
      appliedPromotion: null,
      ageRestricted: null,
    });
    const toast = useToast();
    const v$ = useVuelidate();
    let names = ref<LanguageNamesModal[]>([]);

    let buttondisabled = ref<boolean>(false);
    return {
      languages,
      names,
      keys,
      editproduct,
      categories,
      buttondisabled,
      toast,
      v$,
    };
  },
  validations() {
    return {
      editproduct: {
        name: { required, $autoDirty: true },
        category: { name: { required, $autoDirty: true } },
        barcode: { required, numeric, $autoDirty: true },
        price: { required, numeric, $autoDirty: true },
        weight: { required, numeric, $autoDirty: true },
        image: { required, $autoDirty: true },
      },
    };
  },
  methods: {
    async getNames() {
      this.names = [];
      if (this.product) {
        if (this.product.name) {
          let lang = this.product.name.split("|");
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
        const reader = new FileReader();
        reader.onloadend = async () => {
          const originalImageBase64 = reader.result as string;
          const maxSizeInBytes = 1024 * 10;
          const maxWidth = 720;
          this.editproduct.image = await compressImage(
            originalImageBase64,
            maxSizeInBytes,
            maxWidth
          );
          this.buttondisabled = false;
        };
        reader.readAsDataURL(file);
      }
    },

    async updateProduct() {
      let tempname = "";
      for (let i = 0; this.names.length > i; i++) {
        if (this.names[i]) {
          tempname += `${this.names[i].lang}:${this.names[i].name}|`;
        }
      }
      tempname = tempname.slice(0, -1);
      let data = {
        name: tempname,

        category: 0,
        barcode: this.editproduct.barcode,
        price: this.editproduct.price,
        enabled: this.editproduct.enabled,
        isLoose: this.editproduct.isLoose,
        weight: this.editproduct.weight,
        description: this.editproduct.description,
        image: this.editproduct.image,
        ageRestricted: this.editproduct.ageRestricted,
      };
      if (this.editproduct.category) {
        data = {
          name: tempname,

          category: this.editproduct.category.id as number,
          barcode: this.editproduct.barcode,
          price: this.editproduct.price,
          enabled: this.editproduct.enabled,
          isLoose: this.editproduct.isLoose,
          weight: this.editproduct.weight,
          description: this.editproduct.description,
          image: this.editproduct.image,
          ageRestricted: this.editproduct.ageRestricted,
        };
      }
      if (this.create) {
        try {
          await this.$axios.post(`/product`, data);
          this.toast.success("Utworzono produkt", {
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
          await this.$axios.put(`/product/${this.editproduct.id}`, data);
          this.toast.success("Zedytowano produkt", {
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
      this.editproduct = {
        id: null,
        createDate: null,
        updateDate: null,
        enabled: false,
        name: null,
        barcode: null,
        price: null,
        isLoose: false,
        weight: null,
        description: null,
        category: {
          isVisible: null,
          image: null,
          name: "",
          id: 0,
          parentCategory: null,
          updateDate: null,
          createDate: null,
        },
        image: null,
        discountedPrice: null,
        appliedPromotion: null,
        ageRestricted: null,
      };
      let photo = this.$refs.photo as HTMLInputElement;
      if (photo) photo.value = "";
      this.$emit("refreshproducts", true);
    },
    async getCategories() {
      try {
        await this.$axios
          .get(`/categories?filter=AND&language=all&itemsPerPage=999999&page=1`)
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.categories = resp.response;
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
    async canceladd() {
      this.editproduct = {
        id: null,
        createDate: null,
        updateDate: null,
        enabled: false,
        name: null,
        barcode: null,
        price: null,
        isLoose: false,
        weight: null,
        description: null,
        category: {
          isVisible: null,
          image: null,
          name: "",
          id: 0,
          parentCategory: null,
          updateDate: null,
          createDate: null,
        },
        image: null,
        discountedPrice: null,
        appliedPromotion: null,
        ageRestricted: null,
      };
      let photo = this.$refs.photo as HTMLInputElement;
      if (photo) photo.value = "";
      this.$emit("canceladd", false);
    },
  },
  mounted() {
    this.getNames();
    this.getCategories();
    this.getLanguages();

    this.v$.$validate();
  },
  watch: {
    // whenever question changes, this function will run
    product(value, newvalue) {
      this.getNames();
      this.getLanguages();

      this.getCategories();
      this.editproduct = this.product as ProductModel;
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
