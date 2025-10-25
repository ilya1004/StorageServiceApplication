import {Storekeeper} from '../storekeepers/storekeeper';

export interface Detail {
  id: number;
  nomenclatureCode: string;
  name: string;
  count: number;
  storekeeperId: number;
  storekeeper: Storekeeper | null;
  isDeleted: boolean;
  createdAtDate: string;
  deletedAtDate: string | null;
}
