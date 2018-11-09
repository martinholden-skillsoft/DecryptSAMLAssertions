Contents of openssl.cnf

[req]
distinguished_name = req_distinguished_name
prompt = no
[req_distinguished_name]
CN = hostname.example.org


Generate keypair
.\openssl\openssl req -new -x509 -days 3650 -nodes -sha256 -out saml.crt -keyout saml.pem -config openssl.cnf

Export to PKS
.\openssl\openssl pkcs12 -inkey saml.key -in saml.crt -export -out saml.pfx