<template>
  <div class="row">
    <div class="col-10">Kategorie</div>
    <div class="col-2">
      <div class="btn btn-success" @click="addCategory()">DODAJ</div>
    </div>
  </div>
  <div class="row categoryheader">
    <div class="col-3">Nazwa</div>
    <div class="col-3">Kategoria nadrzędna</div>
    <div class="col-3">Czy widoczna</div>
    <div class="col-1">Edytuj</div>
    <div class="col-1">Usuń</div>
  </div>

  <categories-component
    v-for="(category, index) in categories"
    :key="index"
    :index="index as Number"
    :category="category"
    @getcategoryedited="getcategoryedited"
    @getcategorydeleted="getcategorydeleted"
  ></categories-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component
  ><category-modal
    @canceladd="canceladd"
    @refreshcategories="refreshcategories"
    :create="create"
    :category="category"
  ></category-modal>
  <delete-modal @refresh="refreshcategories" :item="item"></delete-modal>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ResponseModel } from "@/types/Response";
import PaginationComponent from "@/components/PaginationComponent.vue";
import DeleteModal from "@/components/Modals/DeleteModal.vue";
import { showModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import CategoryModal from "@/components/Modals/CategoryModal.vue";
import CategoriesComponent from "@/components/CategoriesComponent.vue";
import { CategoryModel } from "@/types/api/Category";
export default defineComponent({
  name: "CategoriesView",
  components: {
    CategoryModal,
    PaginationComponent,
    CategoriesComponent,
    DeleteModal,
  },
  setup() {
    let categories = ref<CategoryModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let category = ref<CategoryModel>();
    let item = ref<DeleteItemModel>();

    let create = ref<boolean>(false);
    return {
      create,
      item,
      category,
      totalPages,
      categories,
      column,
      itemsPerPage,
      page,
    };
  },
  methods: {
    async getcategoryedited(value: CategoryModel) {
      this.category = value;
    },
    async getcategorydeleted(value: DeleteItemModel) {
      this.item = value;
    },
    async refreshcategories(value: boolean) {
      console.log("refreshcategories", value);
      await this.getCategories();
    },
    async canceladd(value: boolean) {
      this.create = value;
      this.category = {
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
    },
    async addCategory() {
      this.create = true;
      showModal();
    },
    showModal,
    async getCategories() {
      try {
        this.categories = [];
        await this.$axios
          .get(
            `/categories?filter=AND&loadImages=true&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
          )
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.categories = resp.response;

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
    this.getCategories();
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
      this.getCategories();
    }
  },
});
</script>
<style scoped lang="scss">
.categoryheader {
  height: 50px;
  line-height: 50px;
  background-color: #5c5c5c;
}
</style>
