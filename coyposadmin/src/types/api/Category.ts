export interface CategoryModel {
  id: number | null;

  name: string | null;

  parentCategory: CategoryModel | null;

  UpdateDate: Date | string | null;

  CreateDate: Date | string | null;
}
