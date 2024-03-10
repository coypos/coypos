<template>
  <div class="install">
    <h1>Instalacja</h1>
    <div class="page1" v-if="page == 1">
      <div class="row">
        <div class="row">
          <div class="col-12">
            <div class="form-group">
              <label for="image">Wybierz logo sklepu</label>
              <input
                type="file"
                ref="photo"
                accept="image/*"
                @change="encodeImageFileAsURL()"
                class="form-control"
                id="image"
              />
              <div
                class="input-errors"
                v-for="error of v$.image.$errors"
                :key="error.$uid"
              >
                <div class="error-msg">{{ error.$message }}</div>
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-12">
              <img id="base64image" :src="'data:image/png;base64,' + image" />
            </div>
          </div>
          <button
            type="button"
            :class="'btn btn-success'"
            :disabled="buttondisabled || v$.image.$errors.length"
            @click="nextPage"
          >
            Dalej
          </button>
        </div>
      </div>
    </div>
    <div class="page2" v-if="page == 2">
      <div class="row">
        <div class="col-6">
          <div class="row">
            <div class="col-6">
              <label>Kolor tła</label>

              <input
                v-model="backgroundColor"
                type="text"
                class="form-control"
              />
              <label>Kolor tekstu</label>
              <input v-model="textColor" type="text" class="form-control" />
              <label>Kolor obwódki koszyka</label>
              <input
                v-model="cartFirstBackgroundColor"
                type="text"
                class="form-control"
              />
              <label>Kolor tła koszyka koszyka</label>

              <input
                v-model="cartSecondBackgroundColor"
                type="text"
                class="form-control"
              />
              <label>Kolor ramki koszyka</label>

              <input
                v-model="cartBorderColor"
                type="text"
                class="form-control"
              />
              <label>Kolor produktu</label>

              <input v-model="productColor" type="text" class="form-control" />
              <label>Kolor ramki produktu</label>

              <input
                v-model="productColorDarker"
                type="text"
                class="form-control"
              />
              <label>Kolor tekstu produktu</label>

              <input
                v-model="productTextColor"
                type="text"
                class="form-control"
              />
              <label>Kolor przycisku zapłac</label>
              <input v-model="buttonColor" type="text" class="form-control" />
              <label>Kolor ramki przycisku zapłac</label>

              <input
                v-model="buttonColorDarker"
                type="text"
                class="form-control"
              />
              <label>Kolor tekstu przycisku zapłac</label>

              <input
                v-model="buttonTextColor"
                type="text"
                class="form-control"
              />
            </div>
            <div class="col-6">
              <label>Kolor ramki aktywnej flagi</label>

              <input
                v-model="flagBorderColor"
                type="text"
                class="form-control"
              />
              <label>Kolor przycisku pomocy</label>

              <input
                v-model="buttonColorWarning"
                type="text"
                class="form-control"
              />
              <label>Kolor ramki przycisku pomocy</label>

              <input
                v-model="buttonColorWarningDarker"
                type="text"
                class="form-control"
              />
              <label>Kolor teksu przycisku pomocy</label>

              <input
                v-model="buttonWarningTextColor"
                type="text"
                class="form-control"
              />
            </div>
          </div>
        </div>
        <div
          class="col-6 preview"
          :style="'background-color:' + backgroundColor + ';color:' + textColor"
        >
          <div class="row"><div class="col-12">Podgląd</div></div>
          <div class="row">
            <div class="col-12">
              <img
                class="flag"
                src="/images/flag.png"
                alt="Polski"
                id="pl"
                srcset=""
                :style="
                  '      border: 2px solid ' +
                  textColor +
                  '      ;box-shadow: 0px 0px 6px 6px ' +
                  flagBorderColor
                "
              />
            </div>
          </div>
          <div class="row">
            <div class="col-6">
              <div
                class="cart"
                :style="'background-color:' + cartFirstBackgroundColor"
              >
                <span>Twoj koszyk</span>
                <div
                  class="cart-inter"
                  :style="
                    'background-color:' +
                    cartSecondBackgroundColor +
                    ';border: 1px solid ' +
                    cartBorderColor
                  "
                ></div>
              </div>
            </div>
            <div class="col-6">
              <div class="row">
                <div
                  class="category col-4"
                  :style="
                    'background-color:' +
                    productColor +
                    ';color:' +
                    productTextColor +
                    ';border-color:' +
                    productColorDarker
                  "
                >
                  <span>Chleb</span>
                  <img src="/images/category.jpeg" alt="" />
                </div>
                <div
                  class="category col-4"
                  :style="
                    'background-color:' +
                    productColor +
                    ';color:' +
                    productTextColor +
                    ';border-color:' +
                    productColorDarker
                  "
                >
                  <span>Chleb</span>
                  <img src="/images/category.jpeg" alt="" />
                </div>
                <div
                  class="category col-4"
                  :style="
                    'background-color:' +
                    productColor +
                    ';color:' +
                    productTextColor +
                    ';border-color:' +
                    productColorDarker
                  "
                >
                  <span>Chleb</span>
                  <img src="/images/category.jpeg" alt="" />
                </div>
              </div>
              <div class="row">
                <div class="col-12">
                  <div
                    class="pay"
                    :style="
                      'background-color:' +
                      buttonColor +
                      ';color:' +
                      buttonTextColor +
                      ';border-color:' +
                      buttonColorDarker
                    "
                  >
                    <span>Zakoncz i zaplać</span>
                    <img src="/images/card.png" alt="" />
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-12">
                  <div
                    class="warning"
                    :style="
                      'background-color:' +
                      buttonColorWarning +
                      ';color:' +
                      buttonWarningTextColor +
                      ';border-color:' +
                      buttonColorWarningDarker
                    "
                  >
                    <span>Pomoc</span>
                    <img src="/images/chat.png" alt="" />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="row">
        <div class="col-12">
          <button
            type="button"
            :class="'btn btn-success'"
            :disabled="
              v$.textColor.$errors.length ||
              v$.backgroundColor.$errors.length ||
              v$.flagBorderColor.$errors.length ||
              v$.cartFirstBackgroundColor.$errors.length ||
              v$.cartSecondBackgroundColor.$errors.length ||
              v$.cartSecondBackgroundColorDarker.$errors.length ||
              v$.cartBorderColor.$errors.length ||
              v$.productColor.$errors.length ||
              v$.productColorDarker.$errors.length ||
              v$.productTextColor.$errors.length ||
              v$.buttonColor.$errors.length ||
              v$.buttonColorDarker.$errors.length ||
              v$.buttonTextColor.$errors.length ||
              v$.buttonColorWarning.$errors.length ||
              v$.buttonColorWarningDarker.$errors.length ||
              v$.buttonWarningTextColor.$errors.length
            "
            @click="nextPage"
          >
            Zapisz
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { compressImage } from "@/functions";
import { useToast } from "vue-toastification";
import { useVuelidate } from "@vuelidate/core";
import { required } from "@vuelidate/validators";

export default defineComponent({
  name: "InstallView",
  setup() {
    let textColor = ref<string>("#000");
    let backgroundColor = ref<string>("#fff");
    let flagBorderColor = ref<string>("#BDE3FF");
    let cartFirstBackgroundColor = ref<string>("#fff");
    let cartSecondBackgroundColor = ref<string>("#BDE3FF");
    let cartSecondBackgroundColorDarker = ref<string>("#8acdff");
    let cartBorderColor = ref<string>("#ccc");
    let productColor = ref<string>("#BDE3FF");
    let productColorDarker = ref<string>("#8acdff");
    let productTextColor = ref<string>("#000");
    let buttonColor = ref<string>("#14AE5C");
    let buttonColorDarker = ref<string>("#27AE7C");
    let buttonTextColor = ref<string>("#000");
    let buttonColorWarning = ref<string>("#ffa629");
    let buttonColorWarningDarker = ref<string>("#ff9603");
    let buttonWarningTextColor = ref<string>("#000");

    let image = ref<string>("");
    const toast = useToast();
    const v$ = useVuelidate();
    let buttondisabled = ref<boolean>(true);
    let page = ref<number>(1);
    return {
      page,
      buttondisabled,
      image,
      v$,
      toast,
      textColor,
      backgroundColor,
      flagBorderColor,
      cartFirstBackgroundColor,
      cartSecondBackgroundColor,
      cartSecondBackgroundColorDarker,
      cartBorderColor,
      productColor,
      productColorDarker,
      productTextColor,
      buttonColor,
      buttonColorDarker,
      buttonTextColor,
      buttonColorWarning,
      buttonColorWarningDarker,
      buttonWarningTextColor,
    };
  },
  validations() {
    return {
      image: { required, $autoDirty: true },
      textColor: { required, $autoDirty: true },
      backgroundColor: { required, $autoDirty: true },
      flagBorderColor: { required, $autoDirty: true },
      cartFirstBackgroundColor: { required, $autoDirty: true },
      cartSecondBackgroundColor: { required, $autoDirty: true },
      cartSecondBackgroundColorDarker: { required, $autoDirty: true },
      cartBorderColor: { required, $autoDirty: true },
      productColor: { required, $autoDirty: true },
      productColorDarker: { required, $autoDirty: true },
      productTextColor: { required, $autoDirty: true },
      buttonColor: { required, $autoDirty: true },
      buttonColorDarker: { required, $autoDirty: true },
      buttonTextColor: { required, $autoDirty: true },
      buttonColorWarning: { required, $autoDirty: true },
      buttonColorWarningDarker: { required, $autoDirty: true },
      buttonWarningTextColor: { required, $autoDirty: true },
    };
  },
  methods: {
    async nextPage() {
      if (this.page != 2) {
        this.page += 1;
        this.buttondisabled = true;
      } else {
        await this.$axios.post(`/setting`, {
          key: "logo",
          value: this.image,
        });
        await this.$axios.post(`/setting`, {
          key: "--cart-second-background-color",
          value: this.cartSecondBackgroundColor,
        });
        await this.$axios.post(`/setting`, {
          key: "--cart-second-background-color-darker",
          value: this.cartSecondBackgroundColorDarker,
        });
        await this.$axios.post(`/setting`, {
          key: "--cart-border-color",
          value: this.cartBorderColor,
        });
        await this.$axios.post(`/setting`, {
          key: "--text-color",
          value: this.textColor,
        });
        await this.$axios.post(`/setting`, {
          key: "--background-color",
          value: this.backgroundColor,
        });
        await this.$axios.post(`/setting`, {
          key: "--flag-border-color",
          value: this.flagBorderColor,
        });
        await this.$axios.post(`/setting`, {
          key: "--button-color",
          value: this.buttonColor,
        });
        await this.$axios.post(`/setting`, {
          key: "--button-color-darker",
          value: this.buttonColorDarker,
        });
        await this.$axios.post(`/setting`, {
          key: "--button-text-color",
          value: this.buttonTextColor,
        });
        await this.$axios.post(`/setting`, {
          key: "--button-color-warning",
          value: this.buttonColorWarning,
        });
        await this.$axios.post(`/setting`, {
          key: "--button-color-warning-darker",
          value: this.buttonColorWarningDarker,
        });
        await this.$axios.post(`/setting`, {
          key: "--product-color",
          value: this.productColor,
        });
        await this.$axios.post(`/setting`, {
          key: "--product-color-darker",
          value: this.productColorDarker,
        });
        await this.$axios.post(`/setting`, {
          key: "--product-text-color",
          value: this.productTextColor,
        });
        await this.$axios.post(`/setting`, {
          key: "--cart-first-background-color",
          value: this.cartFirstBackgroundColor,
        });
        this.$router.push({ name: "Dashboard" });
      }
    },
    async encodeImageFileAsURL() {
      this.buttondisabled = true;
      const element: HTMLInputElement = this.$refs.photo as HTMLInputElement;
      if (element.files) {
        const file = element.files[0];
        const reader = new FileReader();
        reader.onloadend = async () => {
          const originalImageBase64 = reader.result as string;
          const maxSizeInBytes = 1024 * 10;
          const maxWidth = 720;
          this.image = await compressImage(
            originalImageBase64,
            maxSizeInBytes,
            maxWidth
          );
          this.buttondisabled = false;
        };
        reader.readAsDataURL(file);
      }
    },
  },
});
</script>
<style scoped lang="scss">
.install {
  padding-top: 20px;
  max-width: 90%;

  h1 {
    padding-bottom: 20px;
  }
  .page1 {
    img {
      max-height: 450px;
    }
  }
  .page2 {
    .flag {
      width: 70px;
      height: 40px;
      margin: 2px;
      display: block;
      margin-bottom: 10px;
      margin-top: 15px;
      margin-left: 5px;
      border: 2px solid var(--text-color);
      box-shadow: 0px 0px 6px 6px var(--flag-border-color);
    }
    .preview {
      background-color: white;
      padding: 20px;
      color: black;
      max-height: 400px;
      .cart {
        width: 200px;
        height: 150px;
        border-radius: 20px;
        border: 2px solid black;
        span {
          margin-left: 15px;
        }
        .cart-inter {
          width: 80%;
          height: 70%;
          background-color: #bde3ff;
          border: 1px solid #8acdff;
          border-radius: 20px;

          margin: 0 10% 10%;
        }
      }
      .category {
        background-color: #bde3ff;
        border: 1px solid #8acdff;
        border-radius: 20px;
        width: 30%;
        margin: 2px;
        text-align: center;
        img {
          max-width: 90%;
        }
      }
      .pay {
        width: 98%;
        margin-left: -13px;
        height: 70px;
        background-color: #14ae5c;
        border: 1px solid #27ae7c;
        border-radius: 20px;
        text-align: center;
        img {
          display: block;
          margin-left: 45%;
          height: 40px;
        }
      }
      .warning {
        margin-top: 10px;
        width: 98%;
        margin-left: -13px;
        height: 70px;
        background-color: #14ae5c;
        border: 1px solid #27ae7c;
        border-radius: 20px;
        text-align: center;
        img {
          display: block;
          margin-left: 45%;
          height: 40px;
        }
      }
    }
  }
  .btn {
    width: 100px;
    position: absolute;
    bottom: 100px;
    right: 100px;
  }
}
</style>
