<template>
  <div id="sidebar">
    <div id="sidebar-header">
      <div class="sidebar-buttons">
        <div id="name">Panel administracyjny</div>
        <a
          v-if="this.hidden"
          href="javascript:void(0)"
          class="closebtn"
          @click="openNav()"
          ><i class="fa-solid fa-arrow-right fa-2xl"></i
        ></a>

        <a v-else href="javascript:void(0)" class="closebtn" @click="closeNav()"
          ><i class="fa-solid fa-arrow-left fa-2xl"></i
        ></a>
      </div>
      <div id="logo">
        <img
          src="data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAFA3PEY8MlBGQUZaVVBfeMiCeG5uePWvuZHI////////////////////////////////////////////////////2wBDAVVaWnhpeOuCguv/////////////////////////////////////////////////////////////////////////wAARCALQAtADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwC5RRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRTZXEcZY9qAHEgdSB9aAQehzWU7s7bmOTRHI0ZypxQBq0VD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BXqaC3aU5PC+vrVmO0RMFvmPv0qxQAiqFUBRgCloopDCiiigAooooAKKKKACiiigAooooAKKKKACiiigAopKKAFooooAKKKSgAopaSgAooooAWiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACikooAWkoooAKWkpaACiikoAKKKKAFpKKKAClpKKAFooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKSgAooooAKKKKACiiigBaSlpKACiiigAooooAKKKKAFopKWgAopKKACloooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiikoAWkpaKACiikoAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAWkopaACikpaACiikoAWiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKAEooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAWkoooAKWkpaACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKSloAKKKSgBaSiigBaSiigBaSiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigApaSigBaKSigAooooAWikpaACiiigAooooAKKSloAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACikpaACiiigAooooAKKKKACkpaKACikpaACikooAWiikoAWkoooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACilpKACiiigAooooAKKWkoAKKWkoAKKWigBKWiigAooooAKSlooAKKKKACiiigAooooAKKKKACiiigApKWigBKWiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKAEopaKAEopaKACkpaKAEopaSgAoopaAEopaKAEopaSgAopaSgAooooAKKKKACiiigAooooAKKKKACiiigAoopaAP/9k="
        />
      </div>
    </div>
    <div id="sidebar-content">
      <nav>
        <div class="nav-block">
          <router-link to="/">
            <div class="icon">
              <i class="fa-solid fa-house fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Dashboard</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/products?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-barcode fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Produkty</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/categories?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-book fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Kategorie</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/promotions?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-percent fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Promocje</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/users?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-user fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Użytkownicy</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/employees?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-user-tie fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Pracownicy</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/payment_methods?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-money-bill fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Płatności</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/receipts?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-receipt fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Rachunki</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/languages?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-language fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Języki kas</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/settings?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-gear fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Ustawienia kas</span>
          </router-link>
        </div>
      </nav>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";

export default defineComponent({
  name: "SidebarComponent",
  setup() {
    let hidden = ref<boolean>(false);

    return { hidden };
  },
  methods: {
    async openNav() {
      this.$storage.setStorageSync("hidden", false);
      this.hidden = false;
      const sidebar = document.getElementById("sidebar");
      if (sidebar) {
        sidebar.style.width = "18%";
      }
      const name = document.getElementById("name");

      setTimeout(() => {
        if (name) {
          name.style.display = "inline-block";
        }
      }, 300);
      let icons = document.getElementsByClassName("icon");
      for (let i = 0; icons.length > i; i++) {
        let icon = icons[i] as HTMLElement;
        icon.style.width = "20%";
      }
    },
    async closeNav() {
      this.$storage.setStorageSync("hidden", true);

      this.hidden = true;
      const sidebar = document.getElementById("sidebar");
      if (sidebar) {
        sidebar.style.width = "70px";
      }
      const name = document.getElementById("name");

      if (name) {
        name.style.display = "none";
      }

      let icons = document.getElementsByClassName("icon");
      for (let i = 0; icons.length > i; i++) {
        let icon = icons[i] as HTMLElement;
        icon.style.width = "100%";
      }
    },
  },
  mounted() {
    if (this.$storage.getStorageSync("hidden")) {
      this.closeNav();
    } else {
      this.openNav();
    }
  },
});
</script>
<style scoped lang="scss">
#sidebar {
  position: fixed;
  float: left;
  display: flex;
  flex-direction: column;
  z-index: 10;
  transition: 0.5s;
  left: 0;
  top: 0;
  bottom: 0;
  width: 18%;
  background-color: #1f1f1f;
  border-right: 3px solid #2c2c2c;
  #sidebar-header {
    border-bottom: 3px solid #2c2c2c;
    .sidebar-buttons {
      height: 72px;

      width: 100%;
      display: block;
      border-bottom: 3px solid #2c2c2c;
      text-align: center;
      min-height: 70px;
      line-height: 72px;
      position: relative;
      .closebtn {
        position: absolute;
        top: 0;
        right: 15px;
        display: inline-block;
        color: #fdfdfd;
        margin: auto;
        height: 70px;
      }
      #name {
        position: absolute;
        top: 0;
        left: 5px;
        width: 80%;
        font-size: 1.3em;
        display: inline-block;
        overflow: clip;
        height: 70px;
        margin: auto;
      }
    }
    #logo {
      display: inline-block;
      width: 100%;
      min-height: 70px;
      transition: 1s;
      text-align: center;
      margin: auto;
      line-height: 70px;

      img {
        max-height: 150px;
        width: 30%;
        min-width: 60px;
      }
    }
  }
  #sidebar-content {
    nav {
      .nav-block {
        width: 100%;
        height: 50px;
        font-size: 20px;
        line-height: 50px;
        border-bottom: 3px solid #2c2c2c;

        .icon {
          text-align: center;
          width: 20%;
          height: 100%;
          display: inline-block;
        }
        a {
          width: 100%;
          font-weight: bold;
          height: 100%;
          color: #fdfdfd;
          display: inline-block;
          &.router-link-exact-active {
            color: #6e47d6;
          }
        }
      }
    }
  }
}
</style>
