#!/bin/bash

# DuckDNS Update Script
# Configuración - REEMPLAZA ESTOS VALORES:
DOMAIN="servipuntos-api"  # Tu subdominio sin .duckdns.org
TOKEN="YOUR_TOKEN_HERE"   # Tu token de DuckDNS

# Obtener IP pública actual
PUBLIC_IP=$(curl -s ifconfig.me)
echo "IP pública detectada: $PUBLIC_IP"

# Actualizar DuckDNS
RESPONSE=$(curl -s "https://www.duckdns.org/update?domains=$DOMAIN&token=$TOKEN&ip=$PUBLIC_IP")

if [ "$RESPONSE" = "OK" ]; then
    echo "✅ DuckDNS actualizado exitosamente: $DOMAIN.duckdns.org -> $PUBLIC_IP"
else
    echo "❌ Error actualizando DuckDNS: $RESPONSE"
fi

