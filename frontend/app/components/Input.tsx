import React from 'react';
import {useController, UseControllerProps} from "react-hook-form";
import {Label, TextInput} from "flowbite-react";

type Props = {
    label: string,
    type?: string,
    showLabel?: boolean,
} & UseControllerProps

const Input = (props: Props) => {
    const {fieldState, field} = useController({...props, defaultValue: ''})

    return (
        <div className={`mb-3`}>
            {props.showLabel && (
                <div className={`mb-2 block`}>
                    <Label htmlFor={field.name} value={props.label}></Label>
                </div>
            )}

            <TextInput
                {...props}
                {...field}
                placeholder={props.label}
                type={props.type || 'text'}
                color={fieldState.error ? 'failure' : fieldState.isDirty ? 'success' : undefined}
                helperText={fieldState.error?.message}
                style={{
                    backgroundColor: fieldState.error
                        ? '#fdf2f2' // Light red
                        : fieldState.isDirty
                            ? '#f3faf7' // Light green
                            : 'transparent',
                }}
            />

        </div>
    );
};

export default Input;