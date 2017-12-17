﻿import * as ko from 'knockout';
import * as $ from 'jquery';

interface ITooltipOptions {
    text: KnockoutObservable<string> | string;
    forceOpen: KnockoutObservable<boolean> | boolean;
}

type TooltipOptions = KnockoutObservable<string> | string | ITooltipOptions;

function getTooltipOptions(input: TooltipOptions) {
    const textMaybeString = ko.unwrap<string | ITooltipOptions>(input);
    if (typeof textMaybeString === 'string') {
        return {
            text: input as (KnockoutObservable<string> | string),
            forceOpen: false
        };
    }

    return input as ITooltipOptions;
}


ko.bindingHandlers['tooltip'] = {
    init(element: HTMLElement, valueAccessor: () => TooltipOptions) {
        const $element = $(element);

        ko.computed(() => {
            const options = getTooltipOptions(valueAccessor());
            const text = options && ko.unwrap(options.text);
            if (!text) {
                return;
            }

            const forceOpen = ko.unwrap(options.forceOpen);
            const trigger = forceOpen ? 'manual' : 'hover focus';

            $element.tooltip('dispose');
            $element.tooltip({
                title: text,
                trigger: trigger
            });

            if (forceOpen) {
                $element.tooltip('show');
            }
        }).extend({
            disposeWhenNodeIsRemoved: element
            });

        ko.utils.domNodeDisposal.addDisposeCallback(element, () => {
            const tooltip = $element.data('bs.tooltip'),
                tip = tooltip && tooltip.getTipElement();

            if (tip) {
                $(tip).remove();
            }

            if (tooltip) {
                tooltip.dispose();
            }
        });
    }
};