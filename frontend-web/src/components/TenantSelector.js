import React, { useEffect } from "react";

/**
 * props.tenants: array de { id, nombre, … }
 * props.selectedTenant: el tenant actualmente activo
 * props.onSelect: fn para actualizar tenant
 */
const TenantSelector = ({ tenants, selectedTenant, onSelect }) => {
    // Cuando arriban los tenants, si no hay ninguno seleccionado tomo el primero
    useEffect(() => {
        if (tenants.length > 0 && !selectedTenant) {
            onSelect(tenants[0]);
        }
    }, [tenants, selectedTenant, onSelect]);

    return (
        <select
            value={selectedTenant?.id || ""}
            onChange={e => {
                const t = tenants.find(t => t.id === e.target.value);
                onSelect(t);
            }}
        >
            <option value="" disabled>— Selecciona un tenant —</option>
            {tenants.map(t => (
                <option key={t.id} value={t.id}>
                    {t.nombre}
                </option>
            ))}
        </select>
    );
};

export default TenantSelector;
