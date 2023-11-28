<template>
  <div class="row">
    <div class="col-10">Promocje</div>
    <div class="col-2">
      <div class="btn btn-success" @click="addPromotion()">DODAJ</div>
    </div>
  </div>
  <div class="row header">
    <div class="col-4">Przedmioty promocji</div>
    <div class="col-2">Procent Promocji</div>
    <div class="col-2">Początek Promocji</div>
    <div class="col-2">Koniec Promocji</div>
  </div>

  <promotion-component
    v-for="(promotion, index) in promotions"
    :key="index"
    :index="index"
    :promotion="promotion"
    @getpromotionedited="getpromotionedited"
    @getpromotiondeleted="getpromotiondeleted"
  ></promotion-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component
  ><promotion-modal
    @canceladd="canceladd"
    :create="create"
    :promotion="promotion"
    @refreshpromotions="refreshpromotions"
  ></promotion-modal>
  <delete-modal @refresh="refreshpromotions" :item="item"></delete-modal>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ResponseModel } from "@/types/Response";
import PromotionComponent from "@/components/PromotionsComponent.vue";
import PaginationComponent from "@/components/PaginationComponent.vue";
import DeleteModal from "@/components/Modals/DeleteModal.vue";
import { showModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { PromotionModel } from "@/types/api/Promotion";
import PromotionModal from "@/components/Modals/PromotionModal.vue";

export default defineComponent({
  name: "PromotionView",
  components: {
    PromotionModal,
    PaginationComponent,
    PromotionComponent,
    DeleteModal,
  },
  setup() {
    let promotions = ref<PromotionModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let promotion = ref<PromotionModel>();
    let item = ref<DeleteItemModel>();

    let create = ref<boolean>(false);
    return {
      create,
      item,
      promotion,
      totalPages,
      promotions,
      column,
      itemsPerPage,
      page,
    };
  },
  methods: {
    async getpromotionedited(value: PromotionModel) {
      this.promotion = value;
    },
    async getpromotiondeleted(value: DeleteItemModel) {
      this.item = value;
    },
    async refreshpromotions(value: boolean) {
      console.log("refreshproducts", value);
      await this.getPromotions();
    },
    async canceladd(value: boolean) {
      this.create = value;
      this.promotion = {
        id: null,
        ids: null,
        discountPercentage: null,
        startDate: null,
        endDate: null,
        createDate: null,
        updateDate: null,
      };
    },
    async addPromotion() {
      this.create = true;
      showModal();
    },
    showModal,
    async getPromotions() {
      try {
        await this.$axios
          .get(
            `/promotions?filter=AND&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
          )
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.promotions = resp.response;
            this.totalPages = resp.totalPages;
          });
      } catch (e) {
        console.log(e as string);
      }
    },
  },
  mounted() {
    this.page = parseInt(this.$router.currentRoute.value.query.page as string);
    this.itemsPerPage = parseInt(
      this.$router.currentRoute.value.query.itemsPerPage as string
    );
    this.getPromotions();
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
      this.getPromotions();
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
