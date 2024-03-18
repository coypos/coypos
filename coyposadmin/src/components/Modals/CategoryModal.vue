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
            Edytuj kategorie
          </h1>
          <h1 v-else class="modal-title" id="staticBackdropLabel">
            Dodaj kategorie
          </h1>
        </div>
        <div v-if="editcategory" class="modal-body">
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
                <label for="category">Kategoria nadrzedna</label>
                <select
                  class="form-control selectpicker"
                  id="category"
                  data-live-search="true"
                  v-model="editcategory.parentCategory.name"
                >
                  <option :data-tokens="null">Brak</option>
                  <option
                    :key="category.id"
                    v-for="category in categories"
                    :data-tokens="category.name"
                  >
                    {{ category.name }}
                  </option>
                </select>
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
                  v-for="error of v$.editcategory.image.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
              </div>
            </div>
            <div class="col-6">
              <img
                id="base64image"
                :src="'data:image/png;base64,' + editcategory.image"
              />
            </div>
          </div>
          <div class="row">
            <div class="col-6">
              <div class="form-check">
                <input
                  v-model="editcategory.isVisible"
                  type="checkbox"
                  class="form-check-input"
                  id="isLoose"
                />
                <label class="form-check-label" for="exampleCheck1"
                  >Czy widoczne?</label
                >
              </div>

              <div class="form-check">
                <input
                  v-model="editcategory.enabled"
                  type="checkbox"
                  class="form-check-input"
                  id="enabled"
                />
                <label class="form-check-label" for="exampleCheck1"
                  >Włączony?</label
                >
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
            @click="updateCategory()"
            :disabled="buttondisabled || v$.editcategory.$errors.length"
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
import { CategoryModel } from "@/types/api/Category";
import { LanguageModel } from "@/types/api/Language";

import { ResponseModel } from "@/types/Response";
import { compressImage, resizePNG } from "@/functions";
import { POSITION, useToast } from "vue-toastification";
import { useVuelidate } from "@vuelidate/core";
import { required } from "@vuelidate/validators";
import { LanguageNamesModal } from "@/types/LanguageNames";
export default defineComponent({
  props: {
    category: Object,
    create: Boolean,
  },
  expose: ["showModal"],
  name: "CategoryModal",
  setup() {
    let keys = ref<string[]>();
    let categories = ref<CategoryModel[]>([]);
    let languages = ref<LanguageModel[]>([]);

    let editcategory = ref<CategoryModel>({
      id: null,
      createDate: null,
      updateDate: null,
      name: null,
      isVisible: null,
      parentCategory: {
        name: null,
        isVisible: null,
        id: null,
        image: null,
        parentCategory: null,
        createDate: null,
        updateDate: null,
      },
      image: null,
    });
    let buttondisabled = ref<boolean>(false);
    let names = ref<LanguageNamesModal[]>([]);
    const toast = useToast();
    const v$ = useVuelidate();
    return {
      languages,
      names,
      categories,
      keys,
      editcategory,
      buttondisabled,
      toast,
      v$,
    };
  },
  validations() {
    return {
      editcategory: {
        name: { required, $autoDirty: true },
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
            this.editcategory.image = (
              await resizePNG(file, targetSizeInKB)
            ).substring("data:image/png;base64,".length);
          } else {
            const reader = new FileReader();
            reader.onloadend = async () => {
              const originalImageBase64 = reader.result as string;
              const maxSizeInBytes = 1024 * 10;
              const maxWidth = 720;
              this.editcategory.image = await compressImage(
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

    async updateCategory() {
      let tempname = "";
      for (let i = 0; this.names.length > i; i++) {
        if (this.names[i]) {
          tempname += `${this.names[i].lang}:${this.names[i].name}|`;
        }
      }
      tempname = tempname.slice(0, -1);
      console.log(tempname);
      let result: CategoryModel[] = [];
      if (this.editcategory.parentCategory) {
        if (this.editcategory.parentCategory.name == "Brak") {
          result.push({
            name: null,
            isVisible: null,
            id: null,
            image: null,
            parentCategory: null,
            createDate: null,
            updateDate: null,
          });
        } else {
          result = this.categories.filter((obj: CategoryModel) => {
            if (this.editcategory.parentCategory)
              return obj.name === this.editcategory.parentCategory.name;
          });
        }
      }
      if (!result[0]) {
        result.push({
          name: null,
          isVisible: null,
          id: null,
          image: null,
          parentCategory: null,
          createDate: null,
          updateDate: null,
        });
      }
      let data = {
        name: tempname,
        parentCategory: result[0].id,
        isVisible: this.editcategory.isVisible,
        image: this.editcategory.image,
      };

      if (this.create) {
        try {
          await this.$axios.post(`/category`, data);
          this.toast.success("Utworzono Kategorie", {
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
          await this.$axios.put(`/category/${this.editcategory.id}`, data);
          this.toast.success("Zedytowano kategorie", {
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
      this.editcategory = {
        id: null,
        createDate: null,
        updateDate: null,
        name: null,
        isVisible: null,
        parentCategory: {
          name: null,
          isVisible: null,
          id: null,
          image: null,
          parentCategory: null,
          createDate: null,
          updateDate: null,
        },
        image: null,
      };
      let photo = this.$refs.photo as HTMLInputElement;
      if (photo) photo.value = "";
      this.$emit("refreshcategories", true);
    },
    async getCategories() {
      try {
        await this.$axios
          .get(`/categories?filter=AND&itemsPerPage=999999&page=1`)
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
    async getNames() {
      this.names = [];
      if (this.category) {
        if (this.category.name) {
          let lang = this.category.name.split("|");
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
    async canceladd() {
      this.editcategory = {
        id: null,
        createDate: null,
        updateDate: null,
        name: null,
        isVisible: null,
        parentCategory: {
          name: null,
          isVisible: null,
          id: null,
          image: null,
          parentCategory: null,
          createDate: null,
          updateDate: null,
        },
        image: null,
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
    category(value, newvalue) {
      this.getCategories();
      this.getLanguages();

      this.getNames();

      this.editcategory = this.category as CategoryModel;
      if (this.editcategory.parentCategory == null) {
        this.editcategory.parentCategory = {
          name: null,
          isVisible: null,
          id: null,
          image: null,
          parentCategory: null,
          createDate: null,
          updateDate: null,
        };
      }
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
