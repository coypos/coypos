export interface PaymentMethodModel {
  id: number | null;

  name: string | null;

  image: string | null;

  authData: string | null;

  enabled: boolean | null;
}
