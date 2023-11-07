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
                <label for="name">Nazwa ustawienia</label>
                <input
                  v-model="editsetting.key"
                  class="form-control"
                  id="name"
                />
              </div>
            </div>
            <div class="col-6">
              <div class="form-group">
                <label for="value">Wartosc</label>
                <input
                  v-model="editsetting.value"
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

    return { keys, editsetting };
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
        } catch (e) {
          console.log(e);
        }
      } else {
        try {
          await this.$axios.put(`/setting/${this.editsetting.id}`, data);
        } catch (e) {
          console.log(e);
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
