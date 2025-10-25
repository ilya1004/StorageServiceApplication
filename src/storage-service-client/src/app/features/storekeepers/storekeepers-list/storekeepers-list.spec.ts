import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StorekeepersList } from './storekeepers-list';

describe('StorekeepersList', () => {
  let component: StorekeepersList;
  let fixture: ComponentFixture<StorekeepersList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StorekeepersList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StorekeepersList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
