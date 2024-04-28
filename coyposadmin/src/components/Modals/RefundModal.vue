<template>
  <!-- Modal -->
  <div
    class="modal fade"
    id="deleteModal"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
    tabindex="-1"
    aria-labelledby="staticBackdropLabel"
    aria-hidden="true"
  >
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h1 class="modal-title" id="staticBackdropLabel">
            Czy na pewno chcesz anulować rachunek?
          </h1>
        </div>
        <div class="modal-body" v-if="item">
          Czy na pewno anulować {{ item.name }}?
        </div>
        <div class="modal-footer">
          <button
            type="button"
            :class="'btn btn-success'"
            data-bs-dismiss="modal"
          >
            ANULUJ
          </button>
          <button
            type="button"
            :class="'btn btn-danger'"
            data-bs-dismiss="modal"
            @click="deleteitem()"
          >
            USUŃ
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  props: {
    item: Object,
  },
  expose: ["showModal"],
  name: "DeleteModal",
  setup() {
    let keys = ref<string[]>();
    const toast = useToast();

    return { toast, keys };
  },

  methods: {
    async deleteitem() {
      try {
        if (this.item) {
          await this.$axios
            .post(`/${this.item.what}/${this.item.id}`)
            .then(() => {
              this.$emit("refresh", true);
            });

          this.toast.success("Usunięto", {
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
