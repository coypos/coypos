<template>
  <div :id="'receipt' + index" class="row receipt">
    <div class="col-2">
      <span v-if="receipt.user"
        >{{ receipt.user.email }}, {{ receipt.user.cardNumber }}</span
      >
    </div>
    <div class="col-2">
      <span v-if="receipt.paymentMethod">{{
        receipt.paymentMethod.name.split("|")[0].split(":")[1]
      }}</span>
    </div>
    <div class="col-2">{{ receipt.createDate }}</div>
    <div class="col-5">
      <div
        :key="index"
        v-for="(transaction, index) in receipt.transactions"
        class="row product"
      >
        <div class="col-3">
          {{
            transaction.product.name.split("|")[0].split(":")[1] ||
            transaction.product.name
          }}
        </div>
        <div class="col-3">{{ transaction.quantity }}</div>
        <div class="col-3">{{ transaction.totalPrice }}</div>
        <div class="col-3">{{ transaction.originalPrice }}</div>
      </div>
      <div class="row">
        <div class="col-3"></div>
        <div class="col-3">SUMA</div>
        <div class="col-3">{{ sumProm }}</div>
        <div class="col-3">{{ sum }}</div>
      </div>
    </div>

    <div class="col-1">
      <div
        v-if="receipt.action != 'REFUNDED'"
        class="btn btn-danger"
        @click="deletereceipt()"
      >
        Zwróć
      </div>
      <div v-else>Zwrócono</div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { showModal, showDeleteModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { useToast } from "vue-toastification";

export default defineComponent({
  name: "OneLineComponent",
  props: {
    receipt: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    const toast = useToast();

    return { edit, item, toast };
  },
  computed: {
    sum(): number {
      let temp = 0;

      if (this.receipt) {
        for (let i = 0; this.receipt.transactions.length > i; i++) {
          temp += this.receipt.transactions[i].originalPrice;
        }
      }
      return temp;
    },
    sumProm(): number {
      let temp = 0;

      if (this.receipt) {
        for (let i = 0; this.receipt.transactions.length > i; i++) {
          temp += this.receipt.transactions[i].totalPrice;
        }
      }
      return temp;
    },
  },
  methods: {
    async deletereceipt() {
      if (this.receipt) {
        this.item.id = this.receipt.id;
        this.item.what = "receipt/refund";
        this.item.name = "";
        if (this.receipt.user) this.item.name = this.receipt.user.email;
        showDeleteModal();
        this.$emit("getreceiptdeleted", this.item);
      }
    },

    showModal,
    showDeleteModal,
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "receipt" + this.index
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
.receipt {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
.product {
  border-bottom: 1px solid black;
}
</style>
