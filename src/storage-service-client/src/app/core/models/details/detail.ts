export interface Detail {
  id: number;
  nomenclatureCode: string;
  name: string;
  count: number;
  storekeeperId: number;
  isDeleted: boolean;
  createdAtDate: string;
  deletedAtDate: string | null;
}
