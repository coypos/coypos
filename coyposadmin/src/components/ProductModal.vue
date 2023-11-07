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
            <div class="col-6">
              <div class="form-group">
                <label for="name">Nazwa produktu</label>
                <input
                  v-model="editproduct.name"
                  class="form-control"
                  id="name"
                />
              </div>
            </div>
            <div class="col-6">
              <div class="form-group">
                <label for="category">Kategoria</label>
                <select
                  class="form-control selectpicker"
                  id="category"
                  data-live-search="true"
                  v-model="editproduct.category.name"
                >
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
                <label for="barcode">Kod kreskowy</label>
                <input
                  type="number"
                  v-model="editproduct.barcode"
                  class="form-control"
                  id="barcode"
                />
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
              </div>
            </div>
            <div class="col-6">
              <img
                style="display: block; width: 150px; height: 150px"
                id="base64image"
                :src="editproduct.image"
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
        name: "",
        id: 0,
        parentCategory: null,
        UpdateDate: null,
        CreateDate: null,
      },
      image: null,
    });

    return { keys, editproduct, categories };
  },

  methods: {
    async encodeImageFileAsURL() {
      const element: HTMLInputElement = this.$refs.photo as HTMLInputElement;
      if (element.files) {
        const file = element.files[0];
        const reader = new FileReader();
        reader.onloadend = () => {
          this.editproduct.image = reader.result as string;
        };
        reader.readAsDataURL(file);
      }
    },
    async updateProduct() {
      let result: CategoryModel[] = this.categories.filter(
        (obj: CategoryModel) => {
          if (this.editproduct.category)
            return obj.name === this.editproduct.category.name;
        }
      );

      let data = {
        name: this.editproduct.name,

        category: result[0].id,
        barcode: this.editproduct.barcode,
        price: this.editproduct.price,
        enabled: this.editproduct.enabled,
        isLoose: this.editproduct.isLoose,
        weight: this.editproduct.weight,
        description: this.editproduct.description,
        image: this.editproduct.image,
      };
      if (this.create) {
        try {
          await this.$axios.post(`/product`, data);
        } catch (e) {
          console.log(e);
        }
      } else {
        try {
          await this.$axios.put(`/product/${this.editproduct.id}`, data);
        } catch (e) {
          console.log(e);
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
          name: "",
          id: 0,
          parentCategory: null,
          UpdateDate: null,
          CreateDate: null,
        },
        image: null,
      };
      let photo = this.$refs.photo as HTMLInputElement;
      if (photo) photo.value = "";
      this.$emit("refreshproducts", true);
    },
    async getCategories() {
      try {
        await this.$axios
          .get(`/categories?filter=AND&itemsPerPage=999999&page=1`)
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.categories = resp.response;
          });
      } catch (e) {
        console.log(e as string);
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
          name: "",
          id: 0,
          parentCategory: null,
          UpdateDate: null,
          CreateDate: null,
        },
        image: null,
      };
      let photo = this.$refs.photo as HTMLInputElement;
      if (photo) photo.value = "";
      this.$emit("canceladd", false);
    },
  },
  mounted() {
    this.getCategories();
  },
  watch: {
    // whenever question changes, this function will run
    product(value, newvalue) {
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
