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
            Edytuj pracownika
          </h1>
          <h1 v-else class="modal-title" id="staticBackdropLabel">
            Dodaj pracownika
          </h1>
        </div>
        <div v-if="editemployee" class="modal-body">
          <div class="row">
            <div class="col-3">
              <div class="form-group">
                <label for="value">Nazwa</label>

                <div :class="{ error: v$.editemployee.name.$errors.length }">
                  <input
                    v-model="editemployee.name"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.editemployee.name.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
              </div>
            </div>

            <div class="col-3">
              <div class="form-group">
                <label for="value">Numer karty</label>

                <div :class="{ error: v$.editemployee.cardId.$errors.length }">
                  <input
                    v-model="editemployee.cardId"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.editemployee.cardId.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <label for="value">PIN</label>

                <div :class="{ error: v$.editemployee.pin.$errors.length }">
                  <input
                    v-model="editemployee.pin"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.editemployee.pin.$errors"
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
                <div class="form-check">
                  <input
                    v-model="editemployee.enabled"
                    type="checkbox"
                    class="form-check-input"
                    id="enabled"
                  />
                  <label class="form-check-label" for="exampleCheck1"
                    >Czy aktywny?</label
                  >
                </div>
              </div>
            </div>
            <div class="col-3">
              <div class="form-group">
                <div class="form-check">
                  <input
                    v-model="editemployee.admin"
                    type="checkbox"
                    class="form-check-input"
                    id="enabled"
                  />
                  <label class="form-check-label" for="exampleCheck1"
                    >Czy admin?</label
                  >
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
            :disabled="v$.editemployee.$errors.length"
            type="button"
            :class="'btn btn-success'"
            data-bs-dismiss="modal"
            @click="updateEmployee()"
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
import { EmployeeModel } from "@/types/api/Employee";
import { useVuelidate } from "@vuelidate/core";
import { useToast, POSITION } from "vue-toastification";
import {
  required,
  email,
  minLength,
  numeric,
  helpers,
} from "@vuelidate/validators";

export default defineComponent({
  props: {
    employee: Object,
    create: Boolean,
  },

  expose: ["showModal"],
  name: "EmployeeModal",
  setup() {
    const toast = useToast();
    let keys = ref<string[]>();
    let editemployee = ref<EmployeeModel>({
      id: null,
      name: null,
      cardId: null,
      pin: null,
      enabled: null,
      admin: null,
    });

    const v$ = useVuelidate();
    return { toast, v$, keys, editemployee };
  },
  validations() {
    return {
      editemployee: {
        name: { required, minLength: minLength(3), $autoDirty: true },
        cardId: { required, numeric, $autoDirty: true },
        pin: { required, numeric, $autoDirty: true },
        enabled: { $autoDirty: true },
        admin: { $autoDirty: true },
      },
    };
  },
  methods: {
    async updateEmployee() {
      let data = {
        name: this.editemployee.name,
        cardId: this.editemployee.cardId,
        pin: this.editemployee.pin,
        enabled: this.editemployee.enabled,
        admin: this.editemployee.admin,
      };
      if (this.create) {
        try {
          await this.$axios.post(`/employee`, data);
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
          await this.$axios.put(`/employee/${this.editemployee.id}`, data);
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
      this.editemployee = {
        id: null,
        name: null,
        cardId: null,
        pin: null,
        enabled: null,
        admin: null,
      };
      this.$emit("refreshemployees", true);
    },

    async canceladd() {
      this.editemployee = {
        id: null,
        name: null,
        cardId: null,
        pin: null,
        enabled: null,
        admin: null,
      };

      this.$emit("canceladd", false);
    },
  },
  mounted() {
    this.v$.$validate();
  },
  watch: {
    // whenever question changes, this function will run
    employee(value, newvalue) {
      this.editemployee = this.employee as EmployeeModel;
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
