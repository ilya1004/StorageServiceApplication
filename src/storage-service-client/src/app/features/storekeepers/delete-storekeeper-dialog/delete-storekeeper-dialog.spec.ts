import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteStorekeeperDialog } from './delete-storekeeper-dialog';

describe('DeleteStorekeeperDialog', () => {
  let component: DeleteStorekeeperDialog;
  let fixture: ComponentFixture<DeleteStorekeeperDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteStorekeeperDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteStorekeeperDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
