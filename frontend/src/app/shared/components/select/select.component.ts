import {
  Component,
  ElementRef,
  HostListener,
  afterRenderEffect,
  computed,
  forwardRef,
  inject,
  input,
  signal,
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

// 10-11-2026 Construido por: Ing GAAE - Option rendered by the shared select component.
export interface SelectOption {
  value: unknown;
  label: string;
  disabled?: boolean;
}

// 10-11-2026 Construido por: Ing GAAE - Accessible select that opens its option panel below the control.
@Component({
  selector: 'app-select',
  templateUrl: './select.component.html',
  styleUrl: './select.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectComponent),
      multi: true,
    },
  ],
})
export class SelectComponent implements ControlValueAccessor {
  private static nextInstanceId = 0;

  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);
  private readonly instanceId = SelectComponent.nextInstanceId++;

  readonly options = input<SelectOption[]>([]);
  readonly placeholder = input('Selecciona una opción');
  readonly inputId = input('');

  readonly value = signal<unknown>(null);
  readonly isOpen = signal(false);
  readonly isDisabled = signal(false);
  readonly activeIndex = signal(-1);

  readonly listboxId = `app-select-listbox-${this.instanceId}`;

  readonly selectedLabel = computed(() => {
    const selectedOption = this.options().find((option) => option.value === this.value());
    return selectedOption?.label ?? this.placeholder();
  });

  readonly hasSelectedOption = computed(() =>
    this.options().some((option) => option.value === this.value()),
  );

  private onChange: (value: unknown) => void = () => {};
  private onTouched: () => void = () => {};

  constructor() {
    // Keep the highlighted option visible while navigating with the keyboard.
    afterRenderEffect(() => {
      const index = this.activeIndex();
      if (!this.isOpen() || index < 0) {
        return;
      }

      this.host.nativeElement
        .querySelector<HTMLElement>(`[data-option-index="${index}"]`)
        ?.scrollIntoView({ block: 'nearest' });
    });
  }

  writeValue(value: unknown): void {
    this.value.set(value);
  }

  registerOnChange(fn: (value: unknown) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled.set(isDisabled);
    if (isDisabled) {
      this.close();
    }
  }

  toggle(): void {
    if (this.isDisabled()) {
      return;
    }

    if (this.isOpen()) {
      this.close();
    } else {
      this.open();
    }
  }

  open(): void {
    if (this.isDisabled() || this.isOpen()) {
      return;
    }

    const selectedIndex = this.options().findIndex(
      (option) => option.value === this.value() && !option.disabled,
    );

    this.activeIndex.set(selectedIndex >= 0 ? selectedIndex : this.firstEnabledIndex());
    this.isOpen.set(true);
    this.onTouched();
  }

  close(): void {
    this.isOpen.set(false);
    this.activeIndex.set(-1);
  }

  selectOption(option: SelectOption): void {
    if (option.disabled) {
      return;
    }

    this.value.set(option.value);
    this.onChange(option.value);
    this.close();
  }

  hoverOption(index: number, option: SelectOption): void {
    if (!option.disabled) {
      this.activeIndex.set(index);
    }
  }

  optionId(index: number): string {
    return `${this.listboxId}-${index}`;
  }

  onTriggerKeydown(event: KeyboardEvent): void {
    switch (event.key) {
      case 'ArrowDown':
        event.preventDefault();
        this.isOpen() ? this.moveActive(1) : this.open();
        break;
      case 'ArrowUp':
        event.preventDefault();
        this.isOpen() ? this.moveActive(-1) : this.open();
        break;
      case 'Enter':
      case ' ':
        event.preventDefault();
        this.isOpen() ? this.selectActiveOption() : this.open();
        break;
      case 'Escape':
        if (this.isOpen()) {
          event.preventDefault();
          this.close();
        }
        break;
      case 'Home':
        if (this.isOpen()) {
          event.preventDefault();
          this.setActive(this.firstEnabledIndex());
        }
        break;
      case 'End':
        if (this.isOpen()) {
          event.preventDefault();
          this.setActive(this.lastEnabledIndex());
        }
        break;
      case 'Tab':
        this.close();
        break;
    }
  }

  @HostListener('document:pointerdown', ['$event'])
  onDocumentPointerDown(event: PointerEvent): void {
    const target = event.target as Node | null;
    if (this.isOpen() && target && !this.host.nativeElement.contains(target)) {
      this.close();
    }
  }

  private selectActiveOption(): void {
    const option = this.options()[this.activeIndex()];
    if (option) {
      this.selectOption(option);
    }
  }

  private moveActive(step: number): void {
    const options = this.options();
    if (options.length === 0) {
      return;
    }

    let index = this.activeIndex();
    for (let attempt = 0; attempt < options.length; attempt++) {
      index = (index + step + options.length) % options.length;
      if (!options[index].disabled) {
        break;
      }
    }

    this.setActive(index);
  }

  private setActive(index: number): void {
    if (index >= 0 && index < this.options().length) {
      this.activeIndex.set(index);
    }
  }

  private firstEnabledIndex(): number {
    return this.options().findIndex((option) => !option.disabled);
  }

  private lastEnabledIndex(): number {
    for (let index = this.options().length - 1; index >= 0; index--) {
      if (!this.options()[index].disabled) {
        return index;
      }
    }

    return -1;
  }
}
