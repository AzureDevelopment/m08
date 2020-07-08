const { DefaultAzureCredential } = require('@azure/identity');
const {
    KeyClient,
    KeyVaultKey,
    CryptographyClient,
} = require('@azure/keyvault-keys');
const { SecretClient } = require('@azure/keyvault-secrets');

const getKeyClient = () => {
    const credential = new DefaultAzureCredential();
    const vaultName = 'm08';
    const url = `https://${vaultName}.vault.azure.net`;
    return new KeyClient(url, credential);
};

const getSecretClient = () => {
    const credential = new DefaultAzureCredential();
    const vaultName = 'm08';
    const url = `https://${vaultName}.vault.azure.net`;
    return new SecretClient(url, credential);
};

const encrypt = async (content) => {
    const key = await getKeyClient().getKey('test');
    return Buffer.from(
        (
            await new CryptographyClient(
                key,
                new DefaultAzureCredential()
            ).encrypt('RSA-OAEP-256', Buffer.from(content, 'utf-8'))
        ).result
    ).toString('base64');
};

const decrypt = async (content) => {
    const key = await getKeyClient().getKey('test');
    return Buffer.from(
        (
            await new CryptographyClient(
                key,
                new DefaultAzureCredential()
            ).decrypt('RSA-OAEP-256', Buffer.from(content, 'base64'))
        ).result
    ).toString('utf-8');
};

const wrap = async (keyToWrap, name) => {
    const wrappingKey = await getKeyClient().getKey('test');
    const wrappedKey = Buffer.from(
        (
            await new CryptographyClient(
                wrappingKey,
                new DefaultAzureCredential()
            ).wrapKey('RSA-OAEP-256', Buffer.from(keyToWrap, 'utf-8'))
        ).result
    ).toString('base64');
    const secret = await getSecretClient().setSecret(name, wrappedKey);
    return secret;
};

const unwrap = async (name) => {
    const wrappingKey = await getKeyClient().getKey('test');
    const wrappedKey = getSecretClient().getSecret(name);
    const unwrappedKey = Buffer.from(
        (
            await new CryptographyClient(
                wrappingKey,
                new DefaultAzureCredential()
            ).unwrapKey(
                'RSA-OAEP-256',
                Buffer.from((await wrappedKey).value, 'base64')
            )
        ).result
    ).toString('utf-8');
    return unwrappedKey;
};

const sign = async (fileBytes) => {
    const key = await getKeyClient().getKey('test');
    return Buffer.from(
        (
            await new CryptographyClient(
                key,
                new DefaultAzureCredential()
            ).signData('RS512', fileBytes)
        ).result
    ).toString('base64');
};

const verify = async (fileBytes, signature) => {
    const key = await getKeyClient().getKey('test');
    const result = await new CryptographyClient(
        key,
        new DefaultAzureCredential()
    ).verifyData('RS512', fileBytes, Buffer.from(signature, 'base64'));
    return result;
};

module.exports.encrypt = encrypt;
module.exports.decrypt = decrypt;
module.exports.wrap = wrap;
module.exports.unwrap = unwrap;
module.exports.sign = sign;
module.exports.verify = verify;
