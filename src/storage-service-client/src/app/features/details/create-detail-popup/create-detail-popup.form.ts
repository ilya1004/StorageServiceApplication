import {FormControl} from '@angular/forms';

export interface ICreateDetailForm {
  nomenclatureCode: FormControl<string>;
  name: FormControl<string>;
  count: FormControl<number>;
  storekeeperId: FormControl<number | null>;
  createdAtDate: FormControl<Date | null>;
}
