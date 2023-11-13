<template>
  <div :id="'setting' + index" class="row setting">
    <div class="col-4">{{ setting.key }}</div>
    <div class="col-4">{{ setting.value }}</div>

    <div class="col-1">
      <div class="btn btn-warning" @click="editsetting()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger" @click="deletesetting()">USUŃ</div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { showModal, showDeleteModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";

export default defineComponent({
  name: "OneLineComponent",
  props: {
    setting: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    return { edit, item };
  },
  methods: {
    async editsetting() {
      showModal();
      this.$emit("getsettingedited", this.setting);
    },
    async deletesetting() {
      if (this.setting) {
        this.item.id = this.setting.id;
        this.item.what = "setting";
        this.item.name = this.setting.key;
        showDeleteModal();
        this.$emit("getsettingdeleted", this.item);
      }
    },
    showModal,
    showDeleteModal,
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "setting" + this.index
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
.setting {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
</style>
