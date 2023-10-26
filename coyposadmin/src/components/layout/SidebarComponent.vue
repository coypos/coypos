<template>
  <div id="sidebar">
    <div id="sidebar-header">
      <div v-if="!this.hidden" id="logo">
        <img src="../../assets/logo.png" />
      </div>
      <div v-if="!this.hidden" class="name">Panel administracyjny</div>
      <a
        v-if="this.hidden"
        href="javascript:void(0)"
        id="closebtn"
        @click="openNav()"
        >Open</a
      >

      <a v-else href="javascript:void(0)" class="closebtn" @click="closeNav()"
        >Close</a
      >
    </div>
    <div id="sidebar-content">
      <nav>
        <div class="nav-block">
          <router-link to="/">
            <div class="icon">x</div>
            <span v-show="!this.hidden">Home</span>
          </router-link>
        </div>
        <div class="nav-block">
          <router-link to="/about"
            ><div class="icon">x</div>
            <span v-show="!this.hidden">About</span>
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
      this.hidden = false;
      const sidebar = document.getElementById("sidebar");
      if (sidebar) {
        sidebar.style.width = "18%";
      }

      let icons = document.getElementsByClassName("icon");
      for (let i = 0; icons.length > i; i++) {
        let icon = icons[i] as HTMLElement;
        icon.style.width = "20%";
      }
    },
    async closeNav() {
      this.hidden = true;
      const sidebar = document.getElementById("sidebar");
      if (sidebar) {
        sidebar.style.width = "4%";
      }
      let icons = document.getElementsByClassName("icon");
      for (let i = 0; icons.length > i; i++) {
        let icon = icons[i] as HTMLElement;
        icon.style.width = "100%";
      }
    },
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
    #closebtn {
      display: block;
    }
    #logo {
      display: inline-block;
      width: 25%;
      height: auto;
      transition: 1s;

      img {
        width: 100%;
        height: auto;
      }
    }
    .name {
      width: 75%;
      font-size: 1.3em;
      display: inline-block;
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
