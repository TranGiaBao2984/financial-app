/// <binding />
// ReSharper disable Es6Feature
const merge = require('webpack-merge');

const isProduction = process.env.NODE_ENV === 'production',
    envFile = isProduction ? './webpack.config.prod.js' : './webpack.config.dev.js';

// Include the correct file dependending if we're building for production or not
const commonConfig = require('./webpack.config.common.js'),
    envConfig = require(envFile);

function makeConfig(...params) {
    return merge(commonConfig.makeTargetSpecificConfig(...params), envConfig.makeTargetSpecificConfig(...params));
}

// Export to multiple targets under the 'build' directory:
// - ES5: ultimate fallback
// - ES2015: 'yield' (generators) support
// - ES2017: 'async/await' support
module.exports = [makeConfig('es5'), makeConfig('es6'), makeConfig('es2017')];
