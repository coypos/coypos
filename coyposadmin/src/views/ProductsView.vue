<template>
  <div class="row">
    <div class="col-10">Produkty</div>
    <div class="col-2">
      <div class="btn btn-success" @click="addProduct()">DODAJ</div>
    </div>
  </div>
  <div class="row productheader">
    <div class="col-3">Nazwa</div>
    <div class="col-3">Kategoria</div>
    <div class="col-2">Kod Kreskowy</div>
    <div class="col-2">Cena</div>
    <div class="col-1">Edytuj</div>
    <div class="col-1">Usuń</div>
  </div>

  <product-component
    v-for="(product, index) in products"
    :key="index"
    :index="index as Number"
    :product="product"
    @getproductedited="getproductedited"
    @getproductdeleted="getproductdeleted"
  ></product-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component
  ><product-modal
    @canceladd="canceladd"
    @refreshproducts="refreshproducts"
    :create="create"
    :product="product"
  ></product-modal>
  <delete-modal @refresh="refreshproducts" :item="item"></delete-modal>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ProductModel } from "@/types/api/Product";
import { ResponseModel } from "@/types/Response";
import ProductComponent from "@/components/ProductsComponent.vue";
import PaginationComponent from "@/components/PaginationComponent.vue";
import ProductModal from "@/components/Modals/ProductModal.vue";
import DeleteModal from "@/components/Modals/DeleteModal.vue";
import { showModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "ProductsView",
  components: {
    ProductModal,
    PaginationComponent,
    ProductComponent,
    DeleteModal,
  },
  setup() {
    let products = ref<ProductModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let product = ref<ProductModel>();
    let item = ref<DeleteItemModel>();
    const toast = useToast();
    let search = ref<string>("");
    let create = ref<boolean>(false);
    return {
      search,
      create,
      item,
      product,
      totalPages,
      products,
      column,
      itemsPerPage,
      page,
      toast,
    };
  },
  methods: {
    async getproductedited(value: ProductModel) {
      this.product = value;
    },
    async getproductdeleted(value: DeleteItemModel) {
      this.item = value;
    },
    async refreshproducts(value: boolean) {
      console.log("refreshproducts", value);
      this.create = false;

      await this.getProducts();
    },
    async canceladd(value: boolean) {
      this.create = value;
      this.product = {
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
    },
    async addProduct() {
      this.create = true;
      showModal();
    },
    showModal,
    async getProducts() {
      try {
        this.products = [];
        if (this.$route.query.q) {
          await this.$axios
            .get(
              `/search/${this.$route.query.q}?filter=AND&loadImages=true&language=all&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
            )
            .then((response) => {
              const resp: ResponseModel = response.data;
              this.products = resp.response;

              this.totalPages = resp.totalPages;
            });
        } else {
          await this.$axios
            .get(
              `/products?filter=AND&loadImages=true&language=all&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
            )
            .then((response) => {
              const resp: ResponseModel = response.data;
              this.products = resp.response;

              this.totalPages = resp.totalPages;
            });
        }
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
  },
  mounted() {
    this.page = parseInt(this.$router.currentRoute.value.query.page as string);
    this.itemsPerPage = parseInt(
      this.$router.currentRoute.value.query.itemsPerPage as string
    );
    setInterval(() => {
      if (this.search != (this.$route.query.q as string)) {
        this.search = this.$route.query.q as string;
        this.getProducts();
      }
    }, 200);
    this.getProducts();
  },
  updated() {
    if (
      this.page !=
        parseInt(this.$router.currentRoute.value.query.page as string) ||
      this.itemsPerPage !=
        parseInt(this.$router.currentRoute.value.query.itemsPerPage as string)
    ) {
      this.page = parseInt(
        this.$router.currentRoute.value.query.page as string
      );
      this.itemsPerPage = parseInt(
        this.$router.currentRoute.value.query.itemsPerPage as string
      );
      this.getProducts();
    }
  },
});
</script>
<style scoped lang="scss">
.productheader {
  height: 50px;
  line-height: 50px;
  background-color: #5c5c5c;
}
</style>
