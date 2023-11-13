export interface CategoryModel {
  id: number | null;

  name: string | null;

  parentCategory: CategoryModel | null;
  image: string | null;
  isVisible: boolean | null;

  updateDate: Date | string | null;

  createDate: Date | string | null;
}
