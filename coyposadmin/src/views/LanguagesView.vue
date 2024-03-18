<template>
  <div class="row">
    <div class="col-10">Języki</div>
    <div class="col-2">
      <div class="btn btn-success" @click="addLanguage()">DODAJ</div>
    </div>
  </div>
  <div class="row header">
    <div class="col-2">Nazwa</div>
    <div class="col-2">Skrót</div>
    <div class="col-2">Czy właczony</div>
  </div>

  <language-component
    v-for="(language, index) in languages"
    :key="index"
    :index="index"
    :language="language"
    @getlanguageedited="getlanguageedited"
    @getlanguagedeleted="getlanguagedeleted"
  ></language-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component
  ><language-modal
    @canceladd="canceladd"
    :create="create"
    :language="language"
    @refreshlanguages="refreshlanguages"
  ></language-modal>
  <delete-modal @refresh="refreshlanguages" :item="item"></delete-modal>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ResponseModel } from "@/types/Response";
import LanguageComponent from "@/components/LanguagesComponent.vue";
import PaginationComponent from "@/components/PaginationComponent.vue";
import DeleteModal from "@/components/Modals/DeleteModal.vue";
import { showModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { LanguageModel } from "@/types/api/Language";
import LanguageModal from "@/components/Modals/LanguageModal.vue";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "LanguageView",
  components: {
    LanguageModal,
    PaginationComponent,
    LanguageComponent,
    DeleteModal,
  },
  setup() {
    let languages = ref<LanguageModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let language = ref<LanguageModel>();
    let item = ref<DeleteItemModel>();
    const toast = useToast();

    let create = ref<boolean>(false);
    return {
      toast,
      create,
      item,
      language,
      totalPages,
      languages,
      column,
      itemsPerPage,
      page,
    };
  },
  methods: {
    async getlanguageedited(value: LanguageModel) {
      this.language = value;
    },
    async getlanguagedeleted(value: DeleteItemModel) {
      this.item = value;
    },
    async refreshlanguages(value: boolean) {
      await this.getLanguages();
    },
    async canceladd(value: boolean) {
      this.create = value;
      this.language = {
        id: null,
        name: null,
        image: null,
        enabled: null,
        countryCode: null,
      };
    },
    async addLanguage() {
      this.create = true;
      showModal();
    },
    showModal,
    async getLanguages() {
      try {
        await this.$axios
          .get(
            `/languages?filter=AND&loadImages=true&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
          )
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.languages = resp.response;
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
    this.getLanguages();
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
      this.getLanguages();
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
