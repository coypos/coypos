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
        <img src="../../assets/logo.png" />
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
          <router-link to="/categories"
            ><div class="icon">
              <i class="fa-solid fa-barcode fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Kategorie</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/promotions"
            ><div class="icon">
              <i class="fa-solid fa-barcode fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Promocje</span>
          </router-link>
        </div>

        <div class="nav-block">
          <router-link to="/store_users"
            ><div class="icon">
              <i class="fa-solid fa-barcode fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Pracownicy</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/settings?page=1&itemsPerPage=10"
            ><div class="icon">
              <i class="fa-solid fa-barcode fa-xl"></i>
            </div>
            <span v-show="!this.hidden">Ustawienia Kas</span>
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
