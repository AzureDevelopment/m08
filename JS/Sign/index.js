const multipart = require('parse-multipart');
const { sign } = require('../key-vault-client');

module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');
    const bodyBuffer = Buffer.from(req.body);
    const boundary = multipart.getBoundary(req.headers['content-type']);
    const parts = multipart.Parse(bodyBuffer, boundary);
    context.res = {
        body: {
            signature: await sign(parts[0].data),
        },
    };
};
