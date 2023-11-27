<template>
  <!-- Modal -->
  <div
    class="modal fade"
    id="staticBackdrop"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
    tabindex="-1"
    aria-labelledby="staticBackdropLabel"
    aria-hidden="true"
  >
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h1 v-if="!create" class="modal-title" id="staticBackdropLabel">
            Edytuj użytkownika
          </h1>
          <h1 v-else class="modal-title" id="staticBackdropLabel">
            Dodaj użytkownika
          </h1>
        </div>
        <div v-if="edituser" class="modal-body">
          <div class="row">
            <div class="col-3">
              <div class="form-group">
                <label for="name">Nazwa</label>
                <input v-model="edituser.name" class="form-control" id="name" />
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Rola</label>
                <input
                  v-model="edituser.role"
                  class="form-control"
                  id="value"
                />
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Numer Karty</label>
                <input
                  v-model="edituser.cardNumber"
                  class="form-control"
                  id="value"
                />
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Numer Telefonu</label>
                <input
                  v-model="edituser.phoneNumber"
                  class="form-control"
                  id="value"
                />
              </div>
            </div>
          </div>

          <div class="row">
            <div class="col-3">
              <div class="form-group">
                <label for="name">Punkty</label>
                <input
                  v-model="edituser.points"
                  class="form-control"
                  id="name"
                />
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Email</label>
                <input
                  v-model="edituser.email"
                  class="form-control"
                  id="value"
                />
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Hasło</label>
                <input
                  v-model="edituser.password"
                  class="form-control"
                  id="value"
                />
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">salt</label>
                <input
                  v-model="edituser.salt"
                  class="form-control"
                  id="value"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button
            type="button"
            :class="'btn btn-warning'"
            data-bs-dismiss="modal"
            @click="canceladd()"
          >
            ANULUJ
          </button>
          <button
            type="button"
            :class="'btn btn-success'"
            data-bs-dismiss="modal"
            @click="updateUser()"
          >
            ZAPISZ
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";

import { UserModel } from "@/types/api/User";

export default defineComponent({
  props: {
    user: Object,
    create: Boolean,
  },
  expose: ["showModal"],
  name: "UserModal",
  setup() {
    let keys = ref<string[]>();
    let edituser = ref<UserModel>({
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
    });

    return { keys, edituser };
  },

  methods: {
    async updateUser() {
      let data = {
        name: this.edituser.name,
        role: this.edituser.role,
        cardNumber: this.edituser.cardNumber,
        phoneNumber: this.edituser.phoneNumber,
        points: this.edituser.points,
        email: this.edituser.email,
        password: this.edituser.password,
        salt: this.edituser.salt,
      };
      if (this.create) {
        try {
          await this.$axios.post(`/user`, data);
        } catch (e) {
          console.log(e);
        }
      } else {
        try {
          await this.$axios.put(`/user/${this.edituser.id}`, data);
        } catch (e) {
          console.log(e);
        }
      }
      this.edituser = {
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
      this.$emit("refreshusers", true);
    },

    async canceladd() {
      this.edituser = {
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

      this.$emit("canceladd", false);
    },
  },

  watch: {
    // whenever question changes, this function will run
    user(value, newvalue) {
      this.edituser = this.user as UserModel;
    },
  },
});
</script>
<style scoped lang="scss">
.modal {
  padding-top: 200px;
  --bs-modal-width: 80%;
  .modal-content {
    color: #fdfdfd;
    background-color: #1f1f1f;
    .modal-body {
      font-size: 17px;
      padding: 50px 10px;
    }
    .modal-footer {
      font-size: 20px;
      button {
        font-size: 20px;
        padding: 20px;
      }
    }
  }
}
</style>
