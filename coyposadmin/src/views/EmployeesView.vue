<template>
  <div class="row">
    <div class="col-10">Pracownicy</div>
    <div class="col-2">
      <div class="btn btn-success" @click="addEmployee()">DODAJ</div>
    </div>
  </div>
  <div class="row header">
    <div class="col-2">Nazwa</div>
    <div class="col-2">Numer Karty</div>
    <div class="col-2">Numer PIN</div>
    <div class="col-2">Czy Admin</div>
  </div>

  <employee-component
    v-for="(employee, index) in employees"
    :key="index"
    :index="index"
    :employee="employee"
    @getemployeeedited="getemployeeedited"
    @getemployeedeleted="getemployeedeleted"
  ></employee-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component
  ><employee-modal
    @canceladd="canceladd"
    :create="create"
    :employee="employee"
    @refreshemployees="refreshemployees"
  ></employee-modal>
  <delete-modal @refresh="refreshemployees" :item="item"></delete-modal>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ResponseModel } from "@/types/Response";
import EmployeeComponent from "@/components/EmployeesComponent.vue";
import PaginationComponent from "@/components/PaginationComponent.vue";
import DeleteModal from "@/components/Modals/DeleteModal.vue";
import { showModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { EmployeeModel } from "@/types/api/Employee";
import EmployeeModal from "@/components/Modals/EmployeeModal.vue";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "EmployeeView",
  components: {
    EmployeeModal,
    PaginationComponent,
    EmployeeComponent,
    DeleteModal,
  },
  setup() {
    let employees = ref<EmployeeModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let employee = ref<EmployeeModel>();
    let item = ref<DeleteItemModel>();
    const toast = useToast();

    let create = ref<boolean>(false);
    return {
      toast,
      create,
      item,
      employee,
      totalPages,
      employees,
      column,
      itemsPerPage,
      page,
    };
  },
  methods: {
    async getemployeeedited(value: EmployeeModel) {
      this.employee = value;
    },
    async getemployeedeleted(value: DeleteItemModel) {
      this.item = value;
    },
    async refreshemployees(value: boolean) {
      await this.getEmployees();
      this.create = false;
    },
    async canceladd(value: boolean) {
      this.create = value;
      this.employee = {
        id: null,
        name: null,
        cardId: null,
        pin: null,
        enabled: null,
        admin: null,
      };
    },
    async addEmployee() {
      this.create = true;
      showModal();
    },
    showModal,
    async getEmployees() {
      try {
        await this.$axios
          .get(
            `/employees?filter=AND&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
          )
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.employees = resp.response;
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
    this.getEmployees();
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
      this.getEmployees();
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
