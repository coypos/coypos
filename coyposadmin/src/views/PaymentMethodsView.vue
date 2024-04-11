<template>
  <div class="row">
    <div class="col-10">Metody Płatności</div>
    <div class="col-2">
      <div class="btn btn-success" @click="addPaymentMethod()">DODAJ</div>
    </div>
  </div>
  <div class="row header">
    <div class="col-2">Nazwa</div>
    <div class="col-2">Włączona?</div>
    <div class="col-4">Dodatkowe dane</div>
  </div>

  <payment-method-component
    v-for="(payment_method, index) in payment_methods"
    :key="index"
    :index="index"
    :payment_method="payment_method"
    @getpayment_methodedited="getpayment_methodedited"
    @getpayment_methoddeleted="getpayment_methoddeleted"
  ></payment-method-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component
  ><payment-method-modal
    @canceladd="canceladd"
    :create="create"
    :payment_method="payment_method"
    @refreshpayment_methods="refreshpayment_methods"
  ></payment-method-modal>
  <delete-modal @refresh="refreshpayment_methods" :item="item"></delete-modal>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ResponseModel } from "@/types/Response";
import PaymentMethodComponent from "@/components/PaymentMethodsComponent.vue";
import PaginationComponent from "@/components/PaginationComponent.vue";
import DeleteModal from "@/components/Modals/DeleteModal.vue";
import { showModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { PaymentMethodModel } from "@/types/api/PaymentMethod";
import PaymentMethodModal from "@/components/Modals/PaymentMethodModal.vue";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "PaymentMethodView",
  components: {
    PaymentMethodModal,
    PaginationComponent,
    PaymentMethodComponent,
    DeleteModal,
  },
  setup() {
    let payment_methods = ref<PaymentMethodModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let payment_method = ref<PaymentMethodModel>();
    let item = ref<DeleteItemModel>();
    const toast = useToast();

    let create = ref<boolean>(false);
    return {
      toast,
      create,
      item,
      payment_method,
      totalPages,
      payment_methods,
      column,
      itemsPerPage,
      page,
    };
  },
  methods: {
    async getpayment_methodedited(value: PaymentMethodModel) {
      this.payment_method = value;
    },
    async getpayment_methoddeleted(value: DeleteItemModel) {
      this.item = value;
    },
    async refreshpayment_methods(value: boolean) {
      await this.getPaymentMethods();
      this.create = false;
    },
    async canceladd(value: boolean) {
      this.create = value;
      this.payment_method = {
        id: null,
        name: null,
        image: null,
        authData: null,
        enabled: null,
      };
    },
    async addPaymentMethod() {
      this.create = true;
      showModal();
    },
    showModal,
    async getPaymentMethods() {
      try {
        await this.$axios
          .get(
            `/payment_methods?filter=AND&loadimages=true&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
          )
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.payment_methods = resp.response;
            this.totalPages = resp.totalPages;
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
  },
  mounted() {
    this.page = parseInt(this.$router.currentRoute.value.query.page as string);
    this.itemsPerPage = parseInt(
      this.$router.currentRoute.value.query.itemsPerPage as string
    );
    this.getPaymentMethods();
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
      this.getPaymentMethods();
    }
  },
});
</script>
<style scoped lang="scss">
.header {
  height: 50px;
  line-height: 50px;
  background-color: #5c5c5c;
}
</style>
