<template>
  <div class="row">
    <div class="col-10">Użytkownicy</div>
    <div class="col-2">
      <div class="btn btn-success" @click="addUser()">DODAJ</div>
    </div>
  </div>
  <div class="row header">
    <div class="col-2">Nazwa</div>
    <div class="col-2">Numer Karty</div>
    <div class="col-2">Numer Telefonu</div>
    <div class="col-2">Email</div>
  </div>

  <user-component
    v-for="(user, index) in users"
    :key="index"
    :index="index"
    :user="user"
    @getuseredited="getuseredited"
    @getuserdeleted="getuserdeleted"
  ></user-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component
  ><user-modal
    @canceladd="canceladd"
    :create="create"
    :user="user"
    @refreshusers="refreshusers"
  ></user-modal>
  <delete-modal @refresh="refreshusers" :item="item"></delete-modal>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ResponseModel } from "@/types/Response";
import UserComponent from "@/components/UsersComponent.vue";
import PaginationComponent from "@/components/PaginationComponent.vue";
import DeleteModal from "@/components/Modals/DeleteModal.vue";
import { showModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { UserModel } from "@/types/api/User";
import UserModal from "@/components/Modals/UserModal.vue";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "UserView",
  components: {
    UserModal,
    PaginationComponent,
    UserComponent,
    DeleteModal,
  },
  setup() {
    let users = ref<UserModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let user = ref<UserModel>();
    let item = ref<DeleteItemModel>();
    const toast = useToast();

    let create = ref<boolean>(false);
    return {
      toast,
      create,
      item,
      user,
      totalPages,
      users,
      column,
      itemsPerPage,
      page,
    };
  },
  methods: {
    async getuseredited(value: UserModel) {
      this.user = value;
    },
    async getuserdeleted(value: DeleteItemModel) {
      this.item = value;
    },
    async refreshusers(value: boolean) {
      await this.getUsers();
    },
    async canceladd(value: boolean) {
      this.create = value;
      this.user = {
        id: null,
        name: null,
        role: null,
        cardNumber: null,
        phoneNumber: null,
        points: null,
        email: null,
        password: null,
        salt: null,
        loginToken: null,
        loginTokenValidDate: null,
        createDate: null,
        updateDate: null,
      };
    },
    async addUser() {
      this.create = true;
      showModal();
    },
    showModal,
    async getUsers() {
      try {
        await this.$axios
          .get(
            `/users?filter=AND&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
          )
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.users = resp.response;
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

        console.log(e as string);
      }
    },
  },
  mounted() {
    this.page = parseInt(this.$router.currentRoute.value.query.page as string);
    this.itemsPerPage = parseInt(
      this.$router.currentRoute.value.query.itemsPerPage as string
    );
    this.getUsers();
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
      this.getUsers();
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
