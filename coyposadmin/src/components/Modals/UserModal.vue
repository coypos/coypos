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
                <label for="value">Nazwa</label>

                <div :class="{ error: v$.edituser.name.$errors.length }">
                  <input
                    v-model="edituser.name"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.edituser.name.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
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
            <div v-if="!create" class="col-3">
              <div class="form-group">
                <label for="value">Numer karty</label>

                <div :class="{ error: v$.edituser.cardNumber.$errors.length }">
                  <input
                    v-model="edituser.cardNumber"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.edituser.cardNumber.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Numer telefonu</label>

                <div :class="{ error: v$.edituser.phoneNumber.$errors.length }">
                  <input
                    v-model="edituser.phoneNumber"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.edituser.phoneNumber.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="row">
            <div class="col-3">
              <div class="form-group">
                <label for="value">Punkty</label>

                <div :class="{ error: v$.edituser.points.$errors.length }">
                  <input
                    v-model="edituser.points"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.edituser.points.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Email</label>

                <div :class="{ error: v$.edituser.email.$errors.length }">
                  <input
                    v-model="edituser.email"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.edituser.email.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">Hasło</label>

                <div :class="{ error: v$.edituser.password.$errors.length }">
                  <input
                    v-model="edituser.password"
                    type="password"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.edituser.password.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
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
            :disabled="v$.edituser.$errors.length"
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
import { defineComponent, ref, reactive } from "vue";
import { UserModel } from "@/types/api/User";
import { useVuelidate } from "@vuelidate/core";
import { useToast, POSITION } from "vue-toastification";
import {
  required,
  email,
  minLength,
  numeric,
  helpers,
} from "@vuelidate/validators";

const phoneNumber = helpers.regex(/^(?:[+]{0,1})(?:\d{9,12})?$/);
export default defineComponent({
  props: {
    user: Object,
    create: Boolean,
  },

  expose: ["showModal"],
  name: "UserModal",
  setup() {
    const toast = useToast();
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

    const v$ = useVuelidate();
    return { toast, v$, keys, edituser };
  },
  validations() {
    return {
      edituser: {
        name: { required, minLength: minLength(3), $autoDirty: true },
        cardNumber: { numeric, $autoDirty: true },
        phoneNumber: {
          phoneNumber: helpers.withMessage(
            "This is not valid number",
            phoneNumber
          ),
          $autoDirty: true,
        },
        points: { numeric, $autoDirty: true },
        email: { required, email, $autoDirty: true },
        password: { minLength: minLength(8), $autoDirty: true },
      },
    };
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
      };
      if (this.create) {
        try {
          await this.$axios.post(`/user`, data);
          this.toast.success("Utworzono użytkownika", {
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
      } else {
        try {
          await this.$axios.put(`/user/${this.edituser.id}`, data);
          this.toast.success("Zedytowano użytkownika", {
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
  mounted() {
    this.v$.$validate();
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
      .input-errors {
        color: red;
        font-size: 0.7em;
      }
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
