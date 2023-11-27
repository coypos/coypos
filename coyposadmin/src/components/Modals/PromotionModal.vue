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
            Edytuj promocje
          </h1>
          <h1 v-else class="modal-title" id="staticBackdropLabel">
            Dodaj promocje
          </h1>
        </div>
        <div v-if="editpromotion" class="modal-body">
          <div class="row">
            <div class="col-12">
              <div class="form-group">
                <label for="name">Lista promocyjnych przedmiotów</label>
                <input
                  v-model="editpromotion.ids"
                  class="form-control"
                  id="name"
                />
              </div>
            </div>
            <div class="row">
              <div class="col-2">
                <div class="form-group">
                  <label for="value">Procent</label>
                  <input
                    v-model="editpromotion.discountPercentage"
                    class="form-control"
                    id="value"
                  />
                </div>
              </div>
              <div class="col-3">
                <div class="form-group">
                  <label for="value">Początek Promocji</label>
                  <input
                    v-model="editpromotion.startDate"
                    class="form-control"
                    id="value"
                  />
                </div>
              </div>
              <div class="col-3">
                <div class="form-group">
                  <label for="value">Koniec Promocji</label>
                  <input
                    v-model="editpromotion.endDate"
                    class="form-control"
                    id="value"
                  />
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
            type="button"
            :class="'btn btn-success'"
            data-bs-dismiss="modal"
            @click="updatePromotion()"
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

import { PromotionModel } from "@/types/api/Promotion";

export default defineComponent({
  props: {
    promotion: Object,
    create: Boolean,
  },
  expose: ["showModal"],
  name: "PromotionModal",
  setup() {
    let keys = ref<string[]>();
    let editpromotion = ref<PromotionModel>({
      id: null,
      ids: null,
      discountPercentage: null,
      startDate: null,
      endDate: null,
      createDate: null,
      updateDate: null,
    });

    return { keys, editpromotion };
  },

  methods: {
    async updatePromotion() {
      let data = {
        ids: this.editpromotion.ids,
        discountPercentage: this.editpromotion.discountPercentage,
        startDate: this.editpromotion.startDate,
        endDate: this.editpromotion.endDate,
      };
      if (this.create) {
        try {
          await this.$axios.post(`/promotion`, data);
        } catch (e) {
          console.log(e);
        }
      } else {
        try {
          await this.$axios.put(`/promotion/${this.editpromotion.id}`, data);
        } catch (e) {
          console.log(e);
        }
      }
      this.editpromotion = {
        id: null,
        ids: null,
        discountPercentage: null,
        startDate: null,
        endDate: null,
        createDate: null,
        updateDate: null,
      };
      this.$emit("refreshpromotions", true);
    },

    async canceladd() {
      this.editpromotion = {
        id: null,
        ids: null,
        discountPercentage: null,
        startDate: null,
        endDate: null,
        createDate: null,
        updateDate: null,
      };

      this.$emit("canceladd", false);
    },
  },

  watch: {
    // whenever question changes, this function will run
    promotion(value, newvalue) {
      this.editpromotion = this.promotion as PromotionModel;
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
