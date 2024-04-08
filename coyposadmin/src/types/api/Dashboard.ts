import { CoyposModel } from "@/types/api/Coypos";
import { UserModel } from "@/types/api/User";
import { ProductModel } from "@/types/api/Product";
import { PaymentMethodModel } from "@/types/api/PaymentMethod";
import { ReceiptModel } from "@/types/api/Receipt";
import { PromotionModel } from "@/types/api/Promotion";

export interface DashboardModel {
  /**Server info */
  coypos: CoyposModel;

  /**Total number of employees */
  employee_count: number;

  /**Total number of users */
  user_count: number;

  /**User with most points */
  user_with_most_points: UserModel;

  /**Total number of products */
  product_count: number;

  /**5 most popular products (all time) */
  most_popular_products: ProductModel[];

  /**Total number of categories */
  category_count: number;

  /**Total number of receipts */
  receipt_count: number;

  /**Most recent receipts */
  receipts_count: ReceiptModel[];

  /**Total number of payment methods */
  payment_method_count: number;

  /**5 most popular payment methods (all time) */
  most_popular_payment_methods: PaymentMethodModel[];

  /**Total number of promotions */
  promotion_count: number;

  /**Number of active promotions */
  promotions_active: PromotionModel[];

  /**Revenue from today */
  revenue_today: number;

  /**Revenue from yesterday */
  revenue_yesterday: number;

  /**Gain/loss compared to yesterday */
  revenue_today_trend: number;

  /**Loss on promotions today */
  revenue_today_promotion_loss: number;

  /**Revenue from this week */
  revenue_this_week: number;

  /**Revenue from previous week */
  revenue_previous_week: number;

  /**Gain/loss compared to last week */
  revenue_this_week_trend: number;

  /**Loss on promotions this week */
  revenue_this_week_promotion_loss: number;

  /**Revenue this month */
  revenue_this_month: number;

  /**Revenue previous month */
  revenue_previous_month: number;

  /**Gain/loss compared to last month */
  revenue_this_month_trend: number;

  /**Loss on promotions this month */
  revenue_this_month_promotion_loss: number;

  /**Revenue this year */
  revenue_this_year: number;

  /**Revenue previous year */
  revenue_previous_year: number;
  /**Gain/loss compared to last year */
  revenue_this_year_trend: number;

  /**Loss on promotions this year */
  revenue_this_year_promotion_loss: number;
}
