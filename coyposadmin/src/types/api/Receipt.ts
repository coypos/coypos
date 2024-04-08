import { UserModel } from "@/types/api/User";
import { PaymentMethodModel } from "@/types/api/PaymentMethod";
import { TransactionModel } from "@/types/api/Transaction";

export interface ReceiptModel {
  id: number | null;

  user: UserModel | null;

  action: string | null;

  transactionId: string | null;

  createDate: Date | string | null;

  updateDate: Date | string | null;

  paymentMethod: PaymentMethodModel | null;

  transactions: TransactionModel[];
}
