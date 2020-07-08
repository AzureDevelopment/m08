const { decrypt } = require('../key-vault-client');

module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    context.res = {
        body: await decrypt(req.body.payload),
    };
};
