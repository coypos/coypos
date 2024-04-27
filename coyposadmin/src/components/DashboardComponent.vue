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
          <span class="info-box-icon bg-danger elevation-1"
            ><i class="fas fa-memory"></i
          ></span>
          <span class="info-box-text">&nbsp;Pamięć serwera</span>
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

    <div class="row">
      <div class="col-3">
        <div class="info-box">
          <span class="info-box-icon bg-success elevation-1"
            ><i class="fas fa-percent"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Rożnica</span>

            <span
              v-if="dashboard.revenue_today_trend > 0"
              class="info-box-number text-success"
              >{{ dashboard.revenue_today_trend }}
              {{
                removeInfinity(
                  (
                    (this.dashboard.revenue_today /
                      this.dashboard.revenue_yesterday -
                      1) *
                    100
                  ).toFixed(2)
                )
              }}
            </span>
            <span
              v-if="dashboard.revenue_today_trend < 0"
              class="info-box-number text-danger"
              >{{ dashboard.revenue_today_trend }}
              {{
                removeInfinity(
                  (
                    (this.dashboard.revenue_today /
                      this.dashboard.revenue_yesterday -
                      1) *
                    100
                  ).toFixed(2)
                )
              }}
            </span>
            <span
              v-if="dashboard.revenue_today_trend == 0"
              class="info-box-number"
              >{{ dashboard.revenue_today_trend }} (0 %)
            </span>
          </div>
        </div>
      </div>
      <div class="col-3">
        <div class="info-box">
          <span class="info-box-icon bg-success elevation-1"
            ><i class="fas fa-percent"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Rożnica</span>

            <span
              v-if="dashboard.revenue_this_week_trend > 0"
              class="info-box-number text-success"
              >{{ dashboard.revenue_this_week_trend }}
              {{
                removeInfinity(
                  (
                    (this.dashboard.revenue_this_week /
                      this.dashboard.revenue_previous_week -
                      1) *
                    100
                  ).toFixed(2)
                )
              }}
            </span>
            <span
              v-if="dashboard.revenue_this_week_trend < 0"
              class="info-box-number text-danger"
              >{{ dashboard.revenue_this_week_trend }}
              {{
                removeInfinity(
                  (
                    (this.dashboard.revenue_this_week /
                      this.dashboard.revenue_previous_week -
                      1) *
                    100
                  ).toFixed(2)
                )
              }}
            </span>
            <span
              v-if="dashboard.revenue_this_week_trend == 0"
              class="info-box-number"
              >{{ dashboard.revenue_this_week_trend }} (0 %)
            </span>
          </div>
        </div>
      </div>
      <div class="col-3">
        <div class="info-box">
          <span class="info-box-icon bg-success elevation-1"
            ><i class="fas fa-percent"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Rożnica</span>

            <span
              v-if="dashboard.revenue_this_month_trend > 0"
              class="info-box-number text-success"
              >{{ dashboard.revenue_this_month_trend }}
              {{
                removeInfinity(
                  (
                    (this.dashboard.revenue_this_month /
                      this.dashboard.revenue_previous_month -
                      1) *
                    100
                  ).toFixed(2)
                )
              }}
            </span>
            <span
              v-if="dashboard.revenue_this_month_trend < 0"
              class="info-box-number text-danger"
              >{{ dashboard.revenue_this_month_trend }}
              {{
                removeInfinity(
                  (
                    (this.dashboard.revenue_this_month /
                      this.dashboard.revenue_previous_month -
                      1) *
                    100
                  ).toFixed(2)
                )
              }}
            </span>
            <span
              v-if="dashboard.revenue_this_month_trend == 0"
              class="info-box-number"
              >{{ dashboard.revenue_this_month_trend }} (0 %)
            </span>
          </div>
        </div>
      </div>
      <div class="col-3">
        <div class="info-box">
          <span class="info-box-icon bg-success elevation-1"
            ><i class="fas fa-percent"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Rożnica</span>

            <span
              v-if="dashboard.revenue_this_year_trend > 0"
              class="info-box-number text-success"
              >{{ dashboard.revenue_this_year_trend }}
              {{
                removeInfinity(
                  (
                    (this.dashboard.revenue_this_year /
                      this.dashboard.revenue_previous_year -
                      1) *
                    100
                  ).toFixed(2)
                )
              }}
            </span>
            <span
              v-if="dashboard.revenue_this_year_trend < 0"
              class="info-box-number text-danger"
              >{{ dashboard.revenue_this_year_trend }}
              {{
                removeInfinity(
                  (
                    (this.dashboard.revenue_this_year /
                      this.dashboard.revenue_previous_year -
                      1) *
                    100
                  ).toFixed(2)
                )
              }}
            </span>
            <span
              v-if="dashboard.revenue_this_year_trend == 0"
              class="info-box-number"
              >{{ dashboard.revenue_this_year_trend }} (0 %)
            </span>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-3">
        <Bar class="bar" :data="Bar1data" :options="chartOptions" />
      </div>
      <div class="col-3">
        <Bar class="bar" :data="Bar2data" :options="chartOptions" />
      </div>
      <div class="col-3">
        <Bar class="bar" :data="Bar3data" :options="chartOptions" />
      </div>
      <div class="col-3">
        <Bar class="bar" :data="Bar4data" :options="chartOptions" />
      </div>
    </div>

    <div class="row">
      <div class="col-3">
        <div class="info-box">
          <span class="info-box-icon bg-warning elevation-1"
            ><i class="fas fa-percent"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Różnica na promocjach dzisiaj</span>

            <span class="info-box-number text-warning"
              >{{ dashboard.revenue_today_promotion_loss }}
            </span>
          </div>
        </div>
      </div>
      <div class="col-3">
        <div class="info-box">
          <span class="info-box-icon bg-warning elevation-1"
            ><i class="fas fa-percent"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text"
              >Różnica na promocjach w tym tygodniu</span
            >

            <span class="info-box-number text-warning"
              >{{ dashboard.revenue_this_week_promotion_loss }}
            </span>
          </div>
        </div>
      </div>
      <div class="col-3">
        <div class="info-box">
          <span class="info-box-icon bg-warning elevation-1"
            ><i class="fas fa-percent"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text"
              >Różnica na promocjach w tym miesiacu</span
            >

            <span class="info-box-number text-warning"
              >{{ dashboard.revenue_this_month_promotion_loss }}
            </span>
          </div>
        </div>
      </div>
      <div class="col-3">
        <div class="info-box">
          <span class="info-box-icon bg-warning elevation-1"
            ><i class="fas fa-percent"></i
          ></span>
          <div class="info-box-content">
            <span class="info-box-text">Różnica na promocjach w tym roku</span>

            <span class="info-box-number text-warning"
              >{{ dashboard.revenue_this_year_promotion_loss }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-5">
        <Doughnut id="doughnut" :data="Doughnutdata" :options="chartOptions" />
      </div>
      <div class="col-3">
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
                >{{ product.item.name.split(":")[1].split("|")[0] }}:
                {{ product.value }} sprzedanych <br />
              </span>
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { Pie, Bar, Doughnut } from "vue-chartjs";
import {
  Chart as ChartJS,
  ArcElement,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  BarElement,
} from "chart.js";

ChartJS.register(
  ArcElement,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  BarElement
);

import { DashboardModel } from "@/types/api/Dashboard";

export default defineComponent({
  name: "DashboardComponent",
  components: { Pie, Bar, Doughnut },
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
    removeInfinity(value: number) {
      if (isFinite(value)) {
        return "(" + value + " %)";
      } else {
        return null;
      }
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
    Doughnutdata(): any {
      if (this.dashboard) {
        let labels = [];
        let data = [];
        for (
          let i = 0;
          this.dashboard.payment_methods_most_popular.length > i;
          i++
        ) {
          labels.push(
            this.dashboard.payment_methods_most_popular[i].item.name
              ?.split("|")[0]
              .split(":")[1]
          );
          data.push(this.dashboard.payment_methods_most_popular[i].value);
        }

        return {
          labels: labels,
          datasets: [
            {
              label: "Płatności",
              backgroundColor: ["#41B883", "#E46651", "#00D8FF"],
              data: data,
            },
          ],
        };
      } else
        return {
          labels: ["", "", ""],
          datasets: [
            {
              backgroundColor: ["#41B883", "#E46651", "#00D8FF"],
              data: [0, 0, 0],
            },
          ],
        };
    },
    Bar1data(): any {
      if (this.dashboard) {
        return {
          labels: ["Dzisiaj", "Wczoraj"],
          datasets: [
            {
              label: "Zyski",
              backgroundColor: "#28a745",
              data: [
                this.dashboard.revenue_today,
                this.dashboard.revenue_yesterday,
              ],
            },
          ],
        };
      } else
        return {
          labels: ["Dzisiaj", "Wczoraj"],
          datasets: [
            {
              label: "Zyski",
              backgroundColor: "#28a745",
              data: [0, 0],
            },
          ],
        };
    },
    Bar2data(): any {
      if (this.dashboard) {
        return {
          labels: ["Ten Tydzień", "Zeszły tydzień"],
          datasets: [
            {
              label: "Zyski",
              backgroundColor: "#28a745",
              data: [
                this.dashboard.revenue_this_week,
                this.dashboard.revenue_previous_week,
              ],
            },
          ],
        };
      } else
        return {
          labels: ["Ten Tydzień", "Zeszły tydzień"],
          datasets: [
            {
              label: "Zyski",
              backgroundColor: "#28a745",
              data: [0, 0],
            },
          ],
        };
    },
    Bar3data(): any {
      if (this.dashboard) {
        return {
          labels: ["Ten Miesiąc", "Zeszły Miesiąc"],
          datasets: [
            {
              label: "Zyski",
              backgroundColor: "#28a745",
              data: [
                this.dashboard.revenue_this_month,
                this.dashboard.revenue_previous_month,
              ],
            },
          ],
        };
      } else
        return {
          labels: ["Ten Miesiąc", "Zeszły Miesiąc"],
          datasets: [
            {
              label: "Zyski",
              backgroundColor: "#28a745",
              data: [0, 0],
            },
          ],
        };
    },
    Bar4data(): any {
      if (this.dashboard) {
        return {
          labels: ["Ten Rok", "Zeszły Rok"],
          datasets: [
            {
              label: "Zyski",
              backgroundColor: "#28a745",
              data: [
                this.dashboard.revenue_this_year,
                this.dashboard.revenue_previous_year,
              ],
            },
          ],
        };
      } else
        return {
          labels: ["Ten Rok", "Zeszły Rok"],
          datasets: [
            {
              label: "Zyski",
              backgroundColor: "#28a745",
              data: [0, 0],
            },
          ],
        };
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
        responsive: true,
        color: "#fff",
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
.card {
  background-color: #343a40;
  color: #fff;
}
.content-wrapper {
  background-color: #454d55 !important;
  color: #fff;
}
.table {
  width: 100%;
  margin-bottom: 1rem;
  color: #212529 !important;
}
thead {
  background-color: #454d55 !important;
  color: #fff !important;
  th {
    background-color: #454d55 !important;
    color: #fff !important;
  }
}
tr {
  background-color: #454d55 !important;
  color: #fff !important;
}
td {
  background-color: #454d55 !important;
  color: #fff !important;
}
.bar {
  min-height: 170px;
}
#doughnut {
  min-height: 200px;
}
</style>
