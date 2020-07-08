const { wrap } = require('../key-vault-client');

module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    context.res = {
        // status: 200, /* Defaults to 200 */
        body: await wrap(req.body.payload, req.body.name),
    };
};
