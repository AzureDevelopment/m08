const { encrypt } = require('../key-vault-client');

module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    if (req.body.payload && req.body.payload.length < 245) {
        const encryptedBase64 = await encrypt(req.body.payload);
        context.res = {
            body: encryptedBase64,
        };
    } else {
        context.res = {
            status: 400,
        };
    }
};
