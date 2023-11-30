<template>
  <div class="row">
    <div class="col-10">Ustawienia</div>
    <div class="col-2">
      <div class="btn btn-success" @click="addSetting()">DODAJ</div>
    </div>
  </div>
  <div class="row header">
    <div class="col-4">Nazwa</div>
    <div class="col-4">Wartość</div>
  </div>

  <setting-component
    v-for="(setting, index) in settings"
    :key="index"
    :index="index"
    :setting="setting"
    @getsettingedited="getsettingedited"
    @getsettingdeleted="getsettingdeleted"
  ></setting-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component
  ><setting-modal
    @canceladd="canceladd"
    :create="create"
    :setting="setting"
    @refreshsettings="refreshsettings"
  ></setting-modal>
  <delete-modal @refresh="refreshsettings" :item="item"></delete-modal>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ResponseModel } from "@/types/Response";
import SettingComponent from "@/components/SettingsComponent.vue";
import PaginationComponent from "@/components/PaginationComponent.vue";
import DeleteModal from "@/components/Modals/DeleteModal.vue";
import { showModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { SettingModel } from "@/types/api/Setting";
import SettingModal from "@/components/Modals/SettingModal.vue";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "SettingView",
  components: {
    SettingModal,
    PaginationComponent,
    SettingComponent,
    DeleteModal,
  },
  setup() {
    let settings = ref<SettingModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let setting = ref<SettingModel>();
    let item = ref<DeleteItemModel>();
    const toast = useToast();

    let create = ref<boolean>(false);
    return {
      create,
      item,
      setting,
      totalPages,
      settings,
      column,
      itemsPerPage,
      page,
      toast,
    };
  },
  methods: {
    async getsettingedited(value: SettingModel) {
      this.setting = value;
    },
    async getsettingdeleted(value: DeleteItemModel) {
      this.item = value;
    },
    async refreshsettings(value: boolean) {
      await this.getSettings();
    },
    async canceladd(value: boolean) {
      this.create = value;
      this.setting = {
        id: null,
        key: null,
        value: null,
      };
    },
    async addSetting() {
      this.create = true;
      showModal();
    },
    showModal,
    async getSettings() {
      try {
        await this.$axios
          .get(
            `/settings?filter=AND&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
          )
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.settings = resp.response;
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
    this.getSettings();
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
      this.getSettings();
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
