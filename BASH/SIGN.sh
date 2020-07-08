mkdir ca
cd ca
mkdir ca.db.certs
touch ca.db.index
echo "1234" >ca.db.serial

openssl genrsa -des3 -out ca/ca.key 1024
openssl req -new -x509 -days 10000 -key ca/ca.key -out ca/ca.crt
openssl req -new -newkey rsa:1024 -nodes -keyout mykey.pem -out myreq.pem
openssl ca -config ca.conf -out certificate.pem.crt -infiles myreq.pem


openssl req -x509 -config ca.conf -newkey rsa:4096 -sha256 -nodes -out cacert.pem -outform PEM