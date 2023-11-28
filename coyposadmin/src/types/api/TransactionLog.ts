import { UserModel } from "@/types/api/User";

export interface TransactionLog {
  id: number;

  createdDate: Date | string;

  updatedDate: Date | string;

  userID: number;

  user: UserModel;

  action: string;

  timestamp: Date | string;
}
