export interface ResponseModel {
  response: Array<any>;
  page: number | null;
  totalPages: number;
  itemsPerPage: number | null;
  totalItems: number | null;
  totalItemsFiltered: number | null;
}
