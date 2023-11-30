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
            Edytuj ustawienie
          </h1>
          <h1 v-else class="modal-title" id="staticBackdropLabel">
            Dodaj ustawienie
          </h1>
        </div>
        <div v-if="editsetting" class="modal-body">
          <div class="row">
            <div class="col-6">
              <div class="form-group">
                <label for="value">Nazwa ustawienia</label>

                <div :class="{ error: v$.editsetting.key.$errors.length }">
                  <input
                    v-model="editsetting.key"
                    type="text"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.editsetting.key.$errors"
                    :key="error.$uid"
                  >
                    <div class="error-msg">{{ error.$message }}</div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-6">
              <div class="form-group">
                <label for="value">Wartosc</label>

                <div :class="{ error: v$.editsetting.value.$errors.length }">
                  <input
                    v-model="editsetting.value"
                    type="text"
                    class="form-control"
                    id="value"
                  />
                  <div
                    class="input-errors"
                    v-for="error of v$.editsetting.value.$errors"
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
            :disabled="v$.editsetting.$errors.length"
            type="button"
            :class="'btn btn-success'"
            data-bs-dismiss="modal"
            @click="updateSetting()"
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

import { SettingModel } from "@/types/api/Setting";
import { POSITION, useToast } from "vue-toastification";
import { required, minLength } from "@vuelidate/validators";
import { useVuelidate } from "@vuelidate/core";
export default defineComponent({
  props: {
    setting: Object,
    create: Boolean,
  },
  expose: ["showModal"],
  name: "SettingModal",
  setup() {
    let keys = ref<string[]>();
    let editsetting = ref<SettingModel>({
      id: null,
      key: null,
      value: null,
    });
    const toast = useToast();
    const v$ = useVuelidate();

    return { keys, editsetting, toast, v$ };
  },
  validations() {
    return {
      editsetting: {
        key: { required, minLength: minLength(3), $autoDirty: true },
        value: { required, $autoDirty: true },
      },
    };
  },
  methods: {
    async updateSetting() {
      let data = {
        key: this.editsetting.key,
        value: this.editsetting.value,
      };
      if (this.create) {
        try {
          await this.$axios.post(`/setting`, data);
          this.toast.success("Utworzono ustawienie", {
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
          await this.$axios.put(`/setting/${this.editsetting.id}`, data);
          this.toast.success("Zedytowano ustawienie", {
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
      this.editsetting = {
        id: null,
        key: null,
        value: null,
      };
      this.$emit("refreshsettings", true);
    },

    async canceladd() {
      this.editsetting = {
        id: null,
        key: null,
        value: null,
      };

      this.$emit("canceladd", false);
    },
  },
  mounted() {
    this.v$.$validate();
  },
  watch: {
    // whenever question changes, this function will run
    setting(value, newvalue) {
      this.editsetting = this.setting as SettingModel;
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
