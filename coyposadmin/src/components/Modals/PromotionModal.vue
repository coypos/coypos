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
            Edytuj promocje
          </h1>
          <h1 v-else class="modal-title" id="staticBackdropLabel">
            Dodaj promocje
          </h1>
        </div>
        <div v-if="editpromotion" class="modal-body">
          <div class="row">
            <div class="col-12">
              <div v-for="name in names" :key="name">
                <div>{{ name.name }}</div>
                <button class="btn btn-danger" @click="deleteProduct(name.id)">
                  Usuń
                </button>
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-4">
              <div class="form-group">
                <label for="product">Dodaj produkt do promocji</label>
                <select
                  class="form-control selectpicker"
                  id="product"
                  data-live-search="true"
                  v-model="productToAdd"
                >
                  <option
                    :key="product.id"
                    v-for="product in products"
                    :data-tokens="product.name"
                    :value="product.id"
                  >
                    {{ product.name }}
                  </option>
                </select>
              </div>
            </div>

            <div class="col-4">
              <div>&nbsp;</div>
              <div class="btn btn-success" @click="addProduct()">DODAJ</div>
            </div>
          </div>
          <div class="row">
            <div class="col-2">
              <div class="form-group">
                <label for="value">Procent</label>
                <input
                  v-model="editpromotion.discountPercentage"
                  class="form-control"
                  id="value"
                />
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Początek Promocji</label>
                <VueDatePicker
                  v-model="editpromotion.startDate"
                ></VueDatePicker>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Koniec Promocji</label>
                <VueDatePicker v-model="editpromotion.endDate"></VueDatePicker>
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
            @click="updatePromotion()"
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

import { PromotionModel } from "@/types/api/Promotion";
import { ResponseModel } from "@/types/Response";
import { ProductModel } from "@/types/api/Product";
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
export default defineComponent({
  props: {
    promotion: Object,
    create: Boolean,
  },
  components: { VueDatePicker },
  expose: ["showModal"],
  name: "PromotionModal",
  setup() {
    let keys = ref<string[]>();
    let editpromotion = ref<PromotionModel>({
      id: null,
      ids: null,
      discountPercentage: null,
      startDate: null,
      endDate: null,
      createDate: null,
      updateDate: null,
    });
    let names = ref<object[]>([]);
    let productToAdd = ref<ProductModel>();
    let products = ref<ProductModel[]>([]);

    return { products, productToAdd, names, keys, editpromotion };
  },

  methods: {
    async addProduct() {
      if (this.productToAdd) {
        this.editpromotion.ids += `,${this.productToAdd}`;
        await this.getItemsNames();
      }
    },
    async deleteProduct(id: number) {
      if (this.editpromotion.ids) {
        console.log(id);
        this.editpromotion.ids = this.editpromotion.ids.replace(`${id}`, "");
        this.editpromotion.ids = this.editpromotion.ids.replace(`,,`, ",");
        if (this.editpromotion.ids.endsWith(",")) {
          this.editpromotion.ids = this.editpromotion.ids.slice(0, -1);
        }
        console.log(this.editpromotion.ids);
        await this.getItemsNames();
      }
    },
    async getProducts() {
      try {
        await this.$axios
          .get(`/products?filter=AND&itemsPerPage=999999&page=1`)
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.products = resp.response;
          });
      } catch (e) {
        console.log(e as string);
      }
    },
    async getItemsNames() {
      this.names = [];
      if (this.promotion) {
        let list = [];
        if (this.promotion.ids) {
          list = this.promotion.ids.split(",");
          console.log(list);
          list.forEach((item: string) => {
            try {
              const data = {
                id: item,
              };

              const jsonString = JSON.stringify(data);
              const encodedJsonString = encodeURIComponent(jsonString);

              this.$axios
                .get(
                  `/products?filter=AND&loadImages=false&itemsPerPage=1&page=1&body=${encodedJsonString}`
                )
                .then((response) => {
                  const resp: ResponseModel = response.data;
                  this.names.push({
                    name: resp.response[0].name,
                    id: resp.response[0].id,
                  });
                });
            } catch (e) {
              console.log(e as string);
            }
          });
        }
      }
    },

    async updatePromotion() {
      let data = {
        ids: this.editpromotion.ids,
        discountPercentage: this.editpromotion.discountPercentage,
        startDate: this.editpromotion.startDate,
        endDate: this.editpromotion.endDate,
      };
      if (this.create) {
        try {
          await this.$axios.post(`/promotion`, data);
        } catch (e) {
          console.log(e);
        }
      } else {
        try {
          await this.$axios.put(`/promotion/${this.editpromotion.id}`, data);
        } catch (e) {
          console.log(e);
        }
      }
      this.editpromotion = {
        id: null,
        ids: null,
        discountPercentage: null,
        startDate: null,
        endDate: null,
        createDate: null,
        updateDate: null,
      };
      this.$emit("refreshpromotions", true);
    },

    async canceladd() {
      this.editpromotion = {
        id: null,
        ids: null,
        discountPercentage: null,
        startDate: null,
        endDate: null,
        createDate: null,
        updateDate: null,
      };

      this.$emit("canceladd", false);
    },
  },
  mounted() {
    this.getProducts();
  },

  watch: {
    // whenever question changes, this function will run
    promotion(value, newvalue) {
      this.editpromotion = this.promotion as PromotionModel;
      this.getItemsNames();
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
