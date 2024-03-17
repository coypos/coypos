<template>
  <div class="loginpage">
    <div class="loginbox">
      <div class="login form-group">
        <label for="logininput">Login</label>
        <input
          v-model="login"
          type="text"
          class="form-control"
          id="logininput"
          placeholder="Login"
        />
      </div>
      <div class="password form-group">
        <label for="password">Hasło</label>
        <input
          v-model="password"
          type="password"
          class="form-control"
          id="passwordinput"
          placeholder="Hasło"
        />
      </div>
      <div class="buttons">
        <div @click="loginCheck" class="btn btn-success">Loguj</div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { ResponseModel } from "@/types/Response";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "LoginPageView",
  setup() {
    let login = ref<string>();
    let password = ref<string>();
    const toast = useToast();

    return { toast, login, password };
  },
  methods: {
    async loginCheck() {
      this.$storage.setStorageSync("logged", true);

      try {
        const data = {
          card_id: this.login,
          pin: this.password,
        };

        const jsonString = JSON.stringify(data);
        const encodedJsonString = encodeURIComponent(jsonString);
        await this.$axios
          .get(`/employee_validate_admin?body=${encodedJsonString}`)
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.$storage.setStorageSync("logged", true);
            this.$storage.setStorageSync("username", resp);
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
});
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
.loginpage {
  font-size: 20px;
  .loginbox {
    padding: 30px;
    border: 3px solid #2c2c2c;
    border-radius: 20px;
    position: absolute;
    top: 35%;
    left: 35%;
    right: 35%;
    bottom: 35%;
    min-width: 500px;
    min-height: 300px;
    .form-group {
      padding-top: 15px;
      padding-left: 50px;
      padding-right: 50px;
    }
    .buttons {
      position: absolute;
      bottom: 20px;
      left: 80px;
      right: 80px;
      height: 50px;
      text-align: center;
      .btn {
        font-size: 20px !important;
        width: 200px;
      }
    }
  }
}
</style>
