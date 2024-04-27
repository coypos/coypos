<template>
  <div class="row">
    <div class="col-10">Rachunki</div>
  </div>
  <div class="row header">
    <div class="col-2">Użytkownik</div>
    <div class="col-2">Płatność</div>
    <div class="col-2">Data Płatności</div>
    <div class="col-5">
      <div class="row">
        <div class="col-3">Produkt</div>
        <div class="col-3">Liczba</div>
        <div class="col-3">Cena promocyjna</div>
        <div class="col-3">Cena</div>
      </div>
    </div>
  </div>

  <receipt-component
    v-for="(receipt, index) in receipts"
    :key="index"
    :index="index"
    :receipt="receipt"
    @getreceiptdeleted="getreceiptdeleted"
  ></receipt-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component>
  <refund-modal @refresh="refreshreceipts" :item="item"></refund-modal>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ResponseModel } from "@/types/Response";
import ReceiptComponent from "@/components/ReceiptsComponent.vue";
import PaginationComponent from "@/components/PaginationComponent.vue";
import RefundModal from "@/components/Modals/RefundModal.vue";
import { showModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { ReceiptModel } from "@/types/api/Receipt";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "ReceiptView",
  components: {
    PaginationComponent,
    ReceiptComponent,
    RefundModal,
  },
  setup() {
    let receipts = ref<ReceiptModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let receipt = ref<ReceiptModel>();
    let item = ref<DeleteItemModel>();
    const toast = useToast();

    let create = ref<boolean>(false);
    return {
      toast,
      create,
      item,
      receipt,
      totalPages,
      receipts,
      column,
      itemsPerPage,
      page,
    };
  },
  methods: {
    async getreceiptdeleted(value: DeleteItemModel) {
      this.item = value;
    },
    async refreshreceipts(value: boolean) {
      await this.getReceipts();
      this.create = false;
    },

    showModal,
    async getReceipts() {
      try {
        await this.$axios
          .get(
            `/receipts?filter=AND&language=all&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
          )
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.receipts = resp.response;
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
    this.getReceipts();
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
      this.getReceipts();
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
