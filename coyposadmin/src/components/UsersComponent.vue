<template>
  <div :id="'user' + index" class="row user">
    <div class="col-2">{{ user.name }}</div>
    <div class="col-2">{{ user.cardNumber }}</div>
    <div class="col-2">{{ user.phoneNumber }}</div>
    <div class="col-2">{{ user.email }}</div>
    <div class="col-1">
      <div class="btn btn-info" @click="banuser()">BAN</div>
    </div>
    <div class="col-1">
      <div class="btn btn-primary" @click="unbanuser()">UNBAN</div>
    </div>
    <div class="col-1">
      <div class="btn btn-warning" @click="edituser()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger" @click="deleteuser()">USUŃ</div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { showModal, showDeleteModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "OneLineComponent",
  props: {
    user: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    const toast = useToast();

    return { edit, item, toast };
  },
  methods: {
    async edituser() {
      showModal();
      this.$emit("getuseredited", this.user);
    },
    async deleteuser() {
      if (this.user) {
        this.item.id = this.user.id;
        this.item.what = "user";
        this.item.name = this.user.discountPercentage;
        showDeleteModal();
        this.$emit("getuserdeleted", this.item);
      }
    },
    async banuser() {
      if (this.user) {
        try {
          await this.$axios.post(`/user/${this.user.id}/ban`);
          this.toast.success("Użytkownik zbanowany", {
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
    },
    async unbanuser() {
      if (this.user) {
        try {
          await this.$axios.post(`/user/${this.user.id}/unban`);
          this.toast.success("Użytkownik odbanowany", {
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
    },

    showModal,
    showDeleteModal,
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "user" + this.index
          ) as HTMLElement;
          elem.style.backgroundColor = "#2c2c2c";
        }
      }
    },
  },
  mounted() {
    this.colorBackground();
  },
});
</script>
<style scoped lang="scss">
.user {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
</style>
