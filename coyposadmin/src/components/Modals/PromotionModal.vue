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
                <button
                  class="btn btn-danger"
                  @click="deleteProduct(name.id, name.name)"
                >
                  Usuń
                </button>
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-4">
              <div class="form-group">
                <div
                  class="input-errors"
                  v-for="error of v$.editpromotion.ids.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
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
                    :value="{ id: product.id, name: product.name }"
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
                <div
                  class="input-errors"
                  v-for="error of v$.editpromotion.discountPercentage.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Początek Promocji</label>
                <VueDatePicker
                  v-model="editpromotion.startDate"
                ></VueDatePicker>
                <div
                  class="input-errors"
                  v-for="error of v$.editpromotion.startDate.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Koniec Promocji</label>
                <VueDatePicker v-model="editpromotion.endDate"></VueDatePicker>
                <div
                  class="input-errors"
                  v-for="error of v$.editpromotion.endDate.$errors"
                  :key="error.$uid"
                >
                  <div class="error-msg">{{ error.$message }}</div>
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
            :disabled="v$.editpromotion.$errors.length"
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
import { POSITION, useToast } from "vue-toastification";
import { useVuelidate } from "@vuelidate/core";
import { required, numeric } from "@vuelidate/validators";
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
      affectedProducts: [],
    });
    let names = ref<object[]>([]);
    let productToAdd = ref<ProductModel>();
    let products = ref<ProductModel[]>([]);
    const toast = useToast();
    const v$ = useVuelidate();
    return { products, productToAdd, names, keys, editpromotion, toast, v$ };
  },
  validations() {
    return {
      editpromotion: {
        ids: { required, $autoDirty: true },
        discountPercentage: { required, numeric, $autoDirty: true },
        startDate: { required, $autoDirty: true },
        endDate: { required, $autoDirty: true },
      },
    };
  },
  methods: {
    async addProduct() {
      if (this.productToAdd) {
        this.editpromotion.ids += `,${this.productToAdd.id}`;
        this.names.push(this.productToAdd);
      }
    },
    async deleteProduct(id: number, name: string) {
      if (this.editpromotion.ids) {
        this.editpromotion.ids = this.editpromotion.ids.replace(`${id}`, "");
        this.editpromotion.ids = this.editpromotion.ids.replace(`,,`, ",");
        if (this.editpromotion.ids.endsWith(",")) {
          this.editpromotion.ids = this.editpromotion.ids.slice(0, -1);
        }
        this.names = this.names.filter((name2: any) => name2.name != name);
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
    async getItemsNames() {
      this.names = [];
      if (this.promotion) {
        if (this.promotion.affectedProducts) {
          for (let i = 0; this.promotion.affectedProducts.length > i; i++) {
            this.names.push({
              name: this.promotion.affectedProducts[i].name,
              id: this.promotion.affectedProducts[i].id,
            });
          }
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
          this.toast.success("Utworzono promocje", {
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
          await this.$axios.put(`/promotion/${this.editpromotion.id}`, data);
          this.toast.success("Zedytowano promocje", {
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
      this.editpromotion = {
        id: null,
        ids: null,
        discountPercentage: null,
        startDate: null,
        endDate: null,
        createDate: null,
        updateDate: null,
        affectedProducts: [],
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
        affectedProducts: [],
      };

      this.$emit("canceladd", false);
    },
  },
  mounted() {
    this.getProducts();
    this.v$.$validate();
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
      .input-errors {
        color: red;
        font-size: 0.7em;
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
