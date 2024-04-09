<template>
  <div class="dashboard" v-if="dashboard">
    <div class="row">
      <div class="col-8">
        <div class="row">
          <div class="col-6">
            <div class="info-box">
              <span class="info-box-icon bg-info elevation-1"
                ><i class="fas fa-cog"></i
              ></span>
              <div class="info-box-content">
                <span class="info-box-text">Wersja coypos </span>

                <span class="info-box-number"
                  >{{ dashboard.coypos.version }}
                </span>
              </div>
            </div>
          </div>

          <div class="col-6">
            <div class="info-box mb-3">
              <span class="info-box-icon bg-info elevation-1"
                ><i class="fas fa-thumbs-up"></i
              ></span>
              <div class="info-box-content">
                <span class="info-box-text">Czas działania</span>
                <span class="info-box-number">{{
                  dashboard.coypos.uptime.split(".")[0]
                }}</span>
              </div>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-6">
            <div class="info-box mb-3">
              <span class="info-box-icon bg-info elevation-1"
                ><i class="fas fa-brands fa-ubuntu"></i
              ></span>
              <div class="info-box-content">
                <span class="info-box-text">System</span>
                <span class="info-box-number">{{
                  dashboard.coypos.os_name
                }}</span>
              </div>
            </div>
          </div>
          <div class="col-6">
            <div class="info-box mb-3">
              <span class="info-box-icon bg-info elevation-1"
                ><i class="fas fa-clock"></i
              ></span>
              <div class="info-box-content">
                <span class="info-box-text">Czas serwera</span>
                <span class="info-box-number">{{ dateNow }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-4">
        <div class="info-box mb-3">
          <div class="info-box-content">
            <pie id="memory" :options="chartOptions" :data="chartData" />
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-4">
        <div class="info-box">
          <span class="info-box-icon bg-success elevation-1"
            ><i class="fas fa-user-tie"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Pracowników</span>

            <span class="info-box-number">{{ dashboard.employee_count }} </span>
          </div>
        </div>
      </div>
      <div class="col-4">
        <div class="info-box">
          <span class="info-box-icon bg-success elevation-1"
            ><i class="fas fa-user"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Klientów zarejestrowanych</span>

            <span class="info-box-number">{{ dashboard.user_count }} </span>
          </div>
        </div>
      </div>
      <div class="col-4">
        <div class="info-box">
          <span class="info-box-icon bg-warning elevation-1"
            ><i class="fas fa-percent"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Promocji Dodanych/Aktywnych</span>

            <span class="info-box-number"
              >{{ dashboard.promotion_count }}/{{ promotionsActive }}
            </span>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-4">
        <div class="info-box">
          <span class="info-box-icon bg-success elevation-1"
            ><i class="fas fa-book"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Kategorii</span>

            <span class="info-box-number">{{ dashboard.category_count }} </span>
          </div>
        </div>
      </div>
      <div class="col-4">
        <div class="info-box">
          <span class="info-box-icon bg-success elevation-1"
            ><i class="fas fa-barcode"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Produktów</span>

            <span class="info-box-number">{{ dashboard.product_count }} </span>
          </div>
        </div>
      </div>
      <div class="col-4">
        <div class="info-box">
          <span class="info-box-icon bg-success elevation-1"
            ><i class="fas fa-receipt"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Paragonów</span>

            <span class="info-box-number">{{ dashboard.receipt_count }} </span>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-4">
        <div class="info-box">
          <span class="info-box-icon bg-warning elevation-1"
            ><i class="fas fa-star"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Najwiecej punktów</span>

            <span class="info-box-number"
              >{{ dashboard.user_most_points.name }}
            </span>
          </div>
        </div>
      </div>
      <div class="col-4">
        <div class="info-box">
          <span class="info-box-icon bg-warning elevation-1"
            ><i class="fas fa-star"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Popularne produkty</span>

            <span class="info-box-number">
              <span
                :key="product.id"
                v-for="product in dashboard.products_most_popular"
                >{{ product.item.name }}: {{ product.value }} sztuk <br />
              </span>
            </span>
          </div>
        </div>
      </div>
      <div class="col-4">
        <div class="info-box">
          <span class="info-box-icon bg-warning elevation-1"
            ><i class="fas fa-star"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Popularne metody płatnosci</span>

            <span class="info-box-number">
              <span
                :key="payment.id"
                v-for="payment in dashboard.payment_methods_most_popular"
                >{{ payment.item.name }}: {{ payment.value }} sztuk <br />
              </span>
            </span>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-header border-0">
            <h3 class="card-title">Aktywne promocje</h3>
          </div>
          <div class="card-body table-responsive p-0">
            <table class="table table-striped table-valign-middle">
              <thead>
                <tr>
                  <th>Produkty</th>
                  <th>Promocja</th>
                  <th>Data startu</th>
                  <th>Data zakończenia</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  :key="promotions.id"
                  v-for="promotions in dashboard.promotions_active"
                >
                  <td>{{ promotions.ids }}</td>
                  <td>{{ promotions.discountPercentage }}</td>
                  <td>{{ promotions.startDate }}</td>
                  <td>{{ promotions.endDate }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { Pie } from "vue-chartjs";
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";

ChartJS.register(ArcElement, Tooltip, Legend);

import { DashboardModel } from "@/types/api/Dashboard";

export default defineComponent({
  name: "DashboardComponent",
  components: { Pie },
  setup() {
    let dashboard = ref<DashboardModel>();

    return {
      dashboard,
    };
  },
  methods: {
    async getDashboard() {
      await this.$axios.get(`/dashboard`).then((response) => {
        this.dashboard = response.data;
        console.log(this.dashboard);
      });
    },
  },
  filters: {
    formatDate(value: Date) {
      return new Date(value).toLocaleDateString();
    },
  },
  created() {
    console.log("created");
    this.getDashboard();
  },

  computed: {
    promotionsActive(): number {
      if (this.dashboard) {
        return this.dashboard.promotions_active.length;
      } else {
        return 0;
      }
    },
    dateNow(): string {
      if (this.dashboard) {
        const date = new Date(this.dashboard.coypos.time);
        return (
          "" +
          date.getHours() +
          ":" +
          date.getMinutes() +
          ":" +
          date.getSeconds()
        );
      } else {
        return "";
      }
    },
    chartData(): any {
      if (this.dashboard) {
        let free = this.dashboard.coypos.memory_free;
        let used = this.dashboard.coypos.memory_used;
        return {
          datasets: [
            {
              backgroundColor: ["#DD1B16", "#41B883"],
              data: [used, free],
            },
          ],
        };
      } else
        return {
          labels: ["Memory Used", "Memory Free"],
          datasets: [
            {
              backgroundColor: ["#DD1B16", "#41B883"],
              data: [0, 0],
            },
          ],
        };
    },
    chartOptions() {
      return {
        responsive: false,
      };
    },
  },
});
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
.info-box {
  background-color: #343a40;
  color: #fff;
}
#memory {
  width: 160px;
}
</style>
